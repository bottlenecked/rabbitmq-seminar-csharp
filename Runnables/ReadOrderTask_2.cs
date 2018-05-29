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

        public string Announcement => $"Consuming [{_category}] orders. PRESS CTRL+C TO EXIT";

        public void Run()
        {
            var connection = RabbitConnection.Instance;

            using (var channel = connection.CreateModel())
            {
                //We are back to declaring a static queue to help with round-robbying messages across multiple consumers
                var queueName = $"{_category}_in";
                var queue = channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                const string exchangeName = "orders_direct_exchange";
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true,
                    autoDelete: false, arguments: null);

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
            var txt = $"Received [{_category}] order: {order}";
            //Try to grab the x-special header, and if found append the
            //special request to the printed message. Question: what is the underlying type of special?
            if (args.BasicProperties.IsHeadersPresent() && args.BasicProperties.Headers.TryGetValue("x-special", out var special))
            {
                var specialTxt = Encoding.UTF8.GetString((byte[])special);
                txt += $", special request: {specialTxt}";
            }
            Console.WriteLine(txt);
            Console.WriteLine();
            ((IBasicConsumer)consumer).Model.BasicAck(args.DeliveryTag, multiple: false);
        }
    }
}