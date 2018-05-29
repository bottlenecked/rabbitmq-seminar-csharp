using System;
using System.Collections.Generic;
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

                const string exchangeName = "orders_direct_exchange";
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true,
                    autoDelete: false, arguments: null);

                while (true)
                {
                    var (category, item, special) = GetOrder();
                    var bytes = Encoding.UTF8.GetBytes(item);

                    //we now need to get any special request and place it in our custom "x-special" header

                    var props = channel.CreateBasicProperties();
                    if (!string.IsNullOrEmpty(special))
                    {
                        //you need to set the headers dict, not add because it does not exist.
                        //What other stuff can you set through props? Check VS autocompletion out!
                        props.Headers = new Dictionary<string, object>
                        {
                            {"x-special", special}
                        };
                    }
                    channel.BasicPublish(exchange: exchangeName, routingKey: category,
                        basicProperties: props, body: bytes);
                    Console.WriteLine("Order sent!");
                }
            }
        }

        private static (string Category, string Item, string Special) GetOrder()
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
                        return (Category: order[0], Item: order[1], Special: null);
                    }
                    if (order.Length == 3)
                    {
                        return (Category: order[0], Item: order[1], Special: order[2]);
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
    }
}