using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQSeminar;

namespace RabbitmqSeminar.Runnables
{
    internal class ReadOrderTask : IRunnable
    {
        public string Announcement => "Consume orders from the orders_in queue";

        public void Run()
        {
            //STEP 0: grab the connection
            var connection = RabbitConnection.Instance;

            //STEP 0.1: open a channel in the connection
            using (var channel = connection.CreateModel())
            {
                //STEP 1: declare the orders_in queue. What has now changed is that the queue is declared by the consumer only,
                //since the publisher now publishes to the exchange 'orders_direct_exchange' and not 'directly' to our queue.

                var queue = channel.QueueDeclare("orders_in", durable: true, exclusive: false, autoDelete: false, arguments: null);

                //STEP 1.1: declare the orders_direct_exchange exchange like the publisher did, since we must be sure that the exchange
                //exists before binding our queue to it.
                const string exchangeName = "orders_direct_exchange";
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true,
                    autoDelete: false, arguments: null);

                //STEP 1.2: Now we need to bind the queue to the exchange (we create a binding)
                channel.QueueBind(queue, exchangeName, routingKey: string.Empty, arguments: null);

                //STEP 2: Create the consumer
                var consumer = new EventingBasicConsumer(channel);

                //STEP 3: attach the 'Received' event
                consumer.Received += OnReceived;

                //STEP 4: start consuming from the queue
                channel.BasicConsume(queue, autoAck: false, consumerTag: string.Empty, noLocal: false, exclusive: false,
                    arguments: null, consumer: consumer);

                Console.WriteLine("PRESS ANY KEY TO EXIT");
                Console.ReadKey();
            }
        }

        private static void OnReceived(object consumer, BasicDeliverEventArgs args)
        {
            //STEP 3.1: Parse the message payload
            var order = Encoding.UTF8.GetString(args.Body);
            Console.WriteLine($"Received order: {order}");
            Console.WriteLine();
            //STEP 3.2: Acknowledge the message delivery
            ((IBasicConsumer)consumer).Model.BasicAck(args.DeliveryTag, multiple: false);
        }
    }
}