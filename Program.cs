using System;
using System.Linq;
using RabbitmqSeminar.Runnables;

namespace RabbitMQSeminar
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a task name as parameter, eg. \"lostmessages\"");
                return -1;
            }

            var t = Runnables.GetRunnable(args[0]);
            if (t == null)
            {
                Console.WriteLine($"No task named \"{args[0]}\" found. Please give a valid task name, eg \"lostmessages\"");
                return -1;
            }
            var runnable = Runnables.CreateRunnable(t, args.Skip(1));
            Console.WriteLine(runnable.Announcement);
            runnable.Run();
            return 0;
        }
    }
}
