using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

namespace qASIC.Tools
{
    public static class TypeFinder
    {
        public static List<Type> FindAllTypes<T>()
        {
            var type = typeof(T);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => t != type && type.IsAssignableFrom(t))
                .ToList();
        }

        public static IEnumerable<MethodInfo> FindAllAttributes<T>(BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var atributes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .SelectMany(x => x.GetMethods(bindingFlags))
                .Where(x => x.GetCustomAttributes(typeof(T), false).FirstOrDefault() != null);
            return atributes;
        }
    }
}