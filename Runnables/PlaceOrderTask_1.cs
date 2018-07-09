using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQSeminar;

namespace RabbitmqSeminar.Runnables
{
    internal class PlaceOrderTask : IRunnable
    {
        public string Announcement => "Running place order task. Press CTR+C to exit";

        private readonly ConcurrentDictionary<string, Order> _orders = new ConcurrentDictionary<string, Order>();

        public void Run()
        {
            WelcomeMessage();

            var connection = RabbitConnection.Instance;
            using (var channel = connection.CreateModel())
            {

                const string exchangeName = "orders_direct_exchange";
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true,
                    autoDelete: false, arguments: null);

                //This time, besides placing the order we will need confirmation that our order was actually received and is being prepared.
                //One way to do this is using the RPC pattern (there are other ways we could do this with RabbitMQ though)
                var replyQueueName = ListenForConfirmation(channel);

                while (true)
                {
                    //we need to keep sent messages in a dictionary to be able to identify them on response
                    var order = GetOrder();
                    string correlationId = Guid.NewGuid().ToString();
                    _orders.AddOrUpdate(correlationId, order, (key, prev) => order);

                    var bytes = Encoding.UTF8.GetBytes(order.Item);

                    var props = channel.CreateBasicProperties();
                    //we need to set the reply-to queue and correlation id
                    props.ReplyTo = replyQueueName;
                    props.CorrelationId = correlationId;
                    if (!string.IsNullOrEmpty(order.Special))
                    {
                        props.Headers = new Dictionary<string, object>
                        {
                            {"x-special", order.Special}
                        };
                    }
                    channel.BasicPublish(exchange: exchangeName, routingKey: order.Category,
                        basicProperties: props, body: bytes);
                    Console.WriteLine($"Order sent on {DateTime.UtcNow:T}");
                }
            }
        }

        private static Order GetOrder()
        {
            while (true)
            {
                Console.WriteLine(
                    "Specify a category like \"food:xxx\" OR \"drink:xxx:special preparation request\" to place your order. Type in your preference and press ENTER:");
                Console.WriteLine();
                var order = Console.ReadLine()
                    ?.Split(':')
                    .Select(x => x.Trim())
                    .ToArray();
                if (order != null)
                {
                    if (order.Length == 2)
                    {
                        return new Order {Category = order[0], Item = order[1]};
                    }
                    if (order.Length == 3)
                    {
                        return new Order {Category = order[0], Item = order[1], Special = order[2]};
                    }
                }
                Console.Error.WriteLine("Invalid order.");
            }
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("----------------------");
            Console.WriteLine("Welcome to Burgerrrrr Mania!");
            Console.WriteLine();
        }

        private string ListenForConfirmation(IModel channel)
        {
            //so what we need is to create an anonymous queue and consume from there
            var queueName = channel.QueueDeclare(durable: false, exclusive: true, autoDelete: true);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnConfirmationReceived;
            var _ = channel.BasicConsume(queueName, autoAck: false, consumer: consumer);
            return queueName;
        }

        private void OnConfirmationReceived(object consumer, BasicDeliverEventArgs args)
        {
            var channel = ((IBasicConsumer) consumer).Model;

            //read the corellation id header
            var correlationId = args.BasicProperties.CorrelationId;
            //lookup the dictionary for the message
            if (_orders.TryRemove(correlationId, out var order))
            {
                var serverTime = Encoding.UTF8.GetString(args.Body);
                Console.WriteLine($"Confirmed: order {order} was successfully received on {serverTime}");
            }
            else
            {
                Console.Error.WriteLine($"Received confirmation for unknown order {correlationId}");
            }
            //send ack
            channel.BasicAck(args.DeliveryTag, multiple: false);
        }

        private class Order
        {
            public string Category { get; set; }
            public string Item { get; set; }
            public string Special { get; set; }

            public override string ToString()
            {
                return $"{Category}:{Item}:{Special}";
            }
        }
    }
}