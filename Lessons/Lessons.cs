using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RabbitmqSeminar.Lessons
{
    internal class Lessons
    {
        private static IEnumerable<Type> GetLessons()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass)
                .Where(t => typeof(ILesson).IsAssignableFrom(t));
        }

        public static ILesson GetLesson(string name)
        {
            var lessonType = GetLessons()
                .FirstOrDefault(t => t.Name.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) > -1);
            if (lessonType == null)
            {
                return null;
            }
            var lesson = (ILesson) Activator.CreateInstance(lessonType);
            return lesson;
        }
    }
}
