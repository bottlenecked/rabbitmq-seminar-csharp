using System;
using RabbitmqSeminar.Runnables;

namespace RabbitMQSeminar
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a task name as parameter, eg. \"lesson1\"");
                return -1;
            }

            IRunnable runnable = Runnables.GetRunnable(args[0]);
            if (runnable == null)
            {
                Console.WriteLine($"No task named \"{args[0]}\" found. Please give a valid task name, eg \"lesson1\"");
                return -1;
            }
            Console.WriteLine(runnable.Announcement);
            runnable.Run();
            return 0;
        }
    }
}
