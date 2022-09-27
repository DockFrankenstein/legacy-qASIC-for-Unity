using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

namespace qASIC.Tools
{
    public static class TypeFinder
    {
        private const BindingFlags defaultFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        public static List<Type> FindAllTypesList<T>() =>
            FindAllTypes<T>()
                .ToList();

        public static IEnumerable<Type> FindAllTypes<T>()
        {
            var type = typeof(T);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => t != type && type.IsAssignableFrom(t));
        }

        public static IEnumerable<MethodInfo> FindAllAttributes<T>(BindingFlags bindingFlags = defaultFlags) =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .SelectMany(x => x.GetMethods(bindingFlags))
                .Where(x => x.GetCustomAttributes(typeof(T), false).FirstOrDefault() != null);

        public static IEnumerable<T> CreateConstructorsFromTypes<T>(IEnumerable<Type> types) =>
            types.SelectMany(x =>
            {
                ConstructorInfo constructor = x.GetConstructor(Type.EmptyTypes);
                if (constructor == null || constructor.IsAbstract) return null;
                return new T[] { (T)constructor.Invoke(null) };
            });

        public static List<T> CreateConstructorsFromTypesList<T>(IEnumerable<Type> types) =>
            CreateConstructorsFromTypes<T>(types)
            .ToList();
    }
}