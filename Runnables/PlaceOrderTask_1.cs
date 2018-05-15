using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQSeminar;

namespace RabbitmqSeminar.Runnables
{
    internal class PlaceOrderTask : IRunnable
    {
        public string Announcement => "Running place order task. Press CTR+C to exit";

        public void Run()
        {
            WelcomeMessage();

            // YOUR
            // Add code here that will take an order and will send it to the orders_in queue
            // The queue should be durable but not exclusive/autodeleted

            //First grab the connection and open a channel
            var connection = RabbitConnection.Instance;
            using (var channel = connection.CreateModel())
            {
                //Then create the orders_in queue
                var queue = channel.QueueDeclare("orders_in", exclusive: false, durable: true, autoDelete: false);
                while (true)
                {
                    //read the order and send it to rabbitmq
                    var order = GetOrder();
                    var bytes = Encoding.UTF8.GetBytes(order);
                    channel.BasicPublish(string.Empty, queue, channel.CreateBasicProperties(), bytes);
                }
            }
        }

        private static string GetOrder()
        {
            Console.WriteLine();
            Console.WriteLine("What would you like to order? Type in your preference and press ENTER:");
            Console.WriteLine();
            return Console.ReadLine();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("----------------------");
            Console.WriteLine("Welcome to Burgerrrrr Mania!");
            Console.WriteLine();
        }
    }
}