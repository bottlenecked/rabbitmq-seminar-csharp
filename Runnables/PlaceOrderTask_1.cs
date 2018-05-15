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
            ...
            using (...)
            {
                //Then create the orders_in queue
                var queue = ...
                while (true)
                {
                    //read the order and send it to rabbitmq
                    var order = GetOrder();
                    ...
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
