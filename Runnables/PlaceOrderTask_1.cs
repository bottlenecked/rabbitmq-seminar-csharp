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

            // Add code here that will take an order and will send it to the orders_in queue
            // The queue should be durable but not exclusive/autodeleted

            //First grab the connection and open a channel
            var connection = RabbitConnection.Instance;
            using (var channel = connection.CreateModel())
            {
                //We will now use an exchange to publish messages. When creating a queue it is automatically
                //bound to the default direct exchange, and that is why we were under the illusion that we were publishing
                //'directly' to a queue. What the client does under the covers is publish to the direct ("[empty name]") exchange
                //using the queue name as the routing key.

                //So now instead of declaring a queue, we will be declaring an exchange named 'orders_direct_exchange' and publish there

                const string exchangeName = "orders_direct_exchange";
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true,
                    autoDelete: false, arguments: null);

                while (true)
                {
                    //read the order and send it to rabbitmq
                    var order = GetOrder();
                    var bytes = Encoding.UTF8.GetBytes(order);

                    //publish to the exchange created above using an empty string as routing key.
                    channel.BasicPublish(exchange: exchangeName, routingKey: string.Empty,
                        basicProperties: channel.CreateBasicProperties(), body: bytes);
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