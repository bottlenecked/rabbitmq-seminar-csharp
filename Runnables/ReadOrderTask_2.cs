using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var queueName = $"{_category}_in";
                var queue = channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                const string exchangeName = "orders_direct_exchange";
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true,
                    autoDelete: false, arguments: null);

                channel.QueueBind(queue, exchangeName, routingKey: _category, arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                //introduce an artificial delay to simulate a busy server
                consumer.Received += (o, a) => Task.Delay(TimeSpan.FromSeconds(7)).ContinueWith(_ => OnReceived(o, a));

                channel.BasicConsume(queue, autoAck: false, consumerTag: string.Empty, noLocal: false, exclusive: false,
                    arguments: null, consumer: consumer);

                Console.ReadKey();
            }
        }

        private void OnReceived(object consumer, BasicDeliverEventArgs args)
        {
            var order = Encoding.UTF8.GetString(args.Body);
            var txt = $"Received [{_category}] order: {order}";
            if (args.BasicProperties.IsHeadersPresent() && args.BasicProperties.Headers.TryGetValue("x-special", out var special))
            {
                var specialTxt = Encoding.UTF8.GetString((byte[])special);
                txt += $", special request: {specialTxt}";
            }

            var channel = ((IBasicConsumer) consumer).Model;

            //We now need to send an acknowledgement back to the producer of this message
            Reply(channel, args);

            Console.WriteLine(txt);
            Console.WriteLine();
            channel.BasicAck(args.DeliveryTag, multiple: false);
        }

        private static void Reply(IModel channel, BasicDeliverEventArgs args)
        {
            //grab the reply-to queue name from the headers
            var replyQueue = args.BasicProperties.ReplyTo;
            //also grab the correlation id
            var correlationId = args.BasicProperties.CorrelationId;
            if (!string.IsNullOrEmpty(replyQueue) && !string.IsNullOrEmpty(correlationId))
            {
                //we need to set this message's correlation id to the original correlation id
                var props = channel.CreateBasicProperties();
                props.CorrelationId = correlationId;
                var body = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("T"));
                //and finally publish the response back
                channel.BasicPublish(exchange: string.Empty, routingKey: replyQueue, mandatory: false, basicProperties: props, body: body);
            }
        }
    }
}