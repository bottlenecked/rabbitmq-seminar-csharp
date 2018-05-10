using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RabbitmqSeminar.Runnables
{
    internal class Runnables
    {
        private static IEnumerable<Type> GetRunnables()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass)
                .Where(t => typeof(IRunnable).IsAssignableFrom(t));
        }

        public static IRunnable GetRunnable(string name)
        {
            var lessonType = GetRunnables()
                .FirstOrDefault(t => t.Name.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) > -1);
            if (lessonType == null)
            {
                return null;
            }
            var lesson = (IRunnable) Activator.CreateInstance(lessonType);
            return lesson;
        }
    }
}
