using System;
using System.Linq;
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

            var connection = RabbitConnection.Instance;
            using (var channel = connection.CreateModel())
            {
                //Again, we are going to use a direct exchange, only this time we need to differentiate
                //between food and drinks to help speed up order preparation (cooks use different workbenches from barmen)

                const string exchangeName = "orders_direct_exchange";
                channel.ExchangeDeclare(...);

                while (true)
                {
                    //read the order and send it to rabbitmq
                    var (category, item) = GetOrder();
                    var bytes = Encoding.UTF8.GetBytes(item);

                    //publish to the exchange created above using the item category as the routing key
                    channel.BasicPublish(...);
                    Console.WriteLine("Order sent!");
                }
            }
        }

        private static (string Category, string Item) GetOrder()
        {
            while (true)
            {
                Console.WriteLine(
                    "Specify either \"food:xxx\" OR \"drink:xxx\" to place your order. Type in your preference and press ENTER:");
                Console.WriteLine();
                var order = Console.ReadLine()
                    ?.Split(':')
                    .Select(x => x.Trim())
                    .ToArray();
                if (order != null && order.Length == 2)
                {
                    return (Category: order[0], Item: order[1]);
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
    }
}