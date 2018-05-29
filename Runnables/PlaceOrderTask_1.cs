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
                //Nothing changed in the producer

                const string exchangeName = "orders_direct_exchange";
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true,
                    autoDelete: false, arguments: null);

                while (true)
                {
                    var (category, item) = GetOrder();
                    var bytes = Encoding.UTF8.GetBytes(item);

                    channel.BasicPublish(exchange: exchangeName, routingKey: category,
                        basicProperties: channel.CreateBasicProperties(), body: bytes);
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