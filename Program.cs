using System;
using RabbitmqSeminar.Lessons;

namespace RabbitMQSeminar
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a lesson name as parameter, eg. \"lesson1\"");
                return -1;
            }

            ILesson lesson = Lessons.GetLesson(args[0]);
            if (lesson == null)
            {
                Console.WriteLine($"No lesson named \"{args[0]}\" found. Please give a valid lesson name, eg \"lesson1\"");
                return -1;
            }

            lesson.Run();
            return 0;
        }
    }
}
