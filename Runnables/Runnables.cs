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

        public static Type GetRunnable(string name)
        {
            var runnableType = GetRunnables()
                .FirstOrDefault(t => t.Name.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) > -1);
            return runnableType;
        }

        public static IRunnable CreateRunnable(Type t, IEnumerable<string> args)
        {
            //try to instantiate using a ctor that takes an argument list (from the command line).
            //If that fails, try using the default constructor
            var ctor = t.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(c => c.GetParameters().Any());
            if (ctor != null)
            {
                return (IRunnable) ctor.Invoke(new object[] {args});
            }
            var runnable = (IRunnable) Activator.CreateInstance(t);
            return runnable;
        }
    }
}
