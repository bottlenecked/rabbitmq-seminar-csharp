using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQSeminar;

namespace RabbitmqSeminar.Runnables
{
    internal class ReadOrderTask : IRunnable
    {
        private readonly string _category;

        public ReadOrderTask(IEnumerable<string> args)
        {
            _category = args.FirstOrDefault()?.Trim() ?? string.Empty;
        }

        public string Announcement => $"Consuming [{_category}] orders from the orders_in queue. PRESS CTRL+C TO EXIT";

        public void Run()
        {
            var connection = RabbitConnection.Instance;

            using (var channel = connection.CreateModel())
            {
                //FIRST DIFFERENCE: declare a new '[category]_in' queue as before
                var queueName = $"{_category}_in";
                var queue = channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                //declare the exchange as before
                const string exchangeName = "orders_direct_exchange";
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true,
                    autoDelete: false, arguments: null);

                //HERE IS THE OTHER DIFFERENCE: we need to bind the queue to the exchange using the category
                //as the routing key
                channel.QueueBind(queue, exchangeName, routingKey: _category, arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += OnReceived;

                channel.BasicConsume(queue, autoAck: false, consumerTag: string.Empty, noLocal: false, exclusive: false,
                    arguments: null, consumer: consumer);

                Console.ReadKey();
            }
        }

        private void OnReceived(object consumer, BasicDeliverEventArgs args)
        {
            var order = Encoding.UTF8.GetString(args.Body);
            Console.WriteLine($"Received [{_category}] order: {order}");
            Console.WriteLine();
            ((IBasicConsumer)consumer).Model.BasicAck(args.DeliveryTag, multiple: false);
        }
    }
}