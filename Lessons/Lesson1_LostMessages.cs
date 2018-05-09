using System;
using System.Text;
using System.Threading;
using RabbitMQSeminar;

namespace RabbitmqSeminar.Lessons
{
    /// <summary>
    /// This lesson should make it clear that messages are lost when connection problems occur.
    /// When publishing while the connection is trying to recover the channel will reject calls to
    /// BasicPublish() with exceptions.It is the client's responsibility to either store these messages or
    /// resent them if no confirmation (publisher confirms) is received.
    /// 
    /// To test this behavior run this lesson and stop your local rabbitmq service or disable your network interface
    /// (if connecting to a remote rabbitmq server). The output should look something like this:
    /// 
    /// > dotnet run
    /// successfully published message 1
    /// successfully published message 2
    /// connection closed
    /// publish failed
    /// publish failed
    /// publish failed
    /// publish failed
    /// publish failed
    /// connection recovered
    /// successfully published message 8
    /// successfully published message 9
    /// </summary>
    public class Lesson1_LostMessages : ILesson
    {
        public void Run()
        {
            Console.WriteLine("Runnig lesson1. Press CTRL+C to exit.");
            var connection = RabbitConnection.Instance;
            using (var channel = connection.CreateModel())
            {
                //declare a queue that will be deleted once we disconnect
                var queue = channel.QueueDeclare("testqueue", durable: false, exclusive: true, autoDelete: true, arguments: null);
                int count = 0;
                while (true)
                {
                    try
                    {
                        count++;
                        //each message sent carries a set of useful headers, no need to set something for this case
                        var props = channel.CreateBasicProperties();
                        var message = $"Message no. {count}";
                        var bytes = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("", queue, false, props, bytes);
                        Console.WriteLine($"successfully published message {count}");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("publish failed");
                    }
                    Thread.Sleep(5000);
                }
            }

        }
    }
}
