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
            var connection = ...

            //STEP 0.1: open a channel in the connection
            using (var channel = ...)
            {
                //STEP 1: declare the orders_in queue using THE SAME configuration as before
                //(We need to declare the queue in the consumer as well as the producer!)
                var queue = ...

                //STEP 2: Create the consumer
                var consumer = ...

                //STEP 3: attach the 'Received' event
                consumer.Received += ...

                //STEP 4: start consuming from the queue
                queue.B...

                Console.WriteLine("PRESS ANY KEY TO EXIT");
                Console.ReadKey();
            }
        }

        private static void OnReceived(object consumer, BasicDeliverEventArgs args)
        {
            //STEP 3.1: Parse the message payload
            var order = ...
            Console.WriteLine($"Received order: {order}");
            Console.WriteLine();
            //STEP 3.2: Acknowledge the message delivery
            ((IBasicConsumer)consumer).Model.B...
        }
    }
}
