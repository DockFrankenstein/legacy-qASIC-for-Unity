using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace qASIC
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

        public static IEnumerable<MethodInfo> FindAllAttributes<T>(BindingFlags bindingFlags = defaultFlags)
            where T : Attribute =>
            FindAllAttributes(typeof(T), bindingFlags);

        public static IEnumerable<MethodInfo> FindAllAttributes(Type type, BindingFlags bindingFlags = defaultFlags) =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .SelectMany(x => x.GetMethods(bindingFlags))
                .Where(x => x.GetCustomAttributes(type, false).FirstOrDefault() != null);

        public static IEnumerable<FieldInfo> FindAllFieldAttributesInClass<TClass, TAttribute>(BindingFlags bindingFlags = defaultFlags)
            where TClass : class
            where TAttribute : Attribute =>
            FindAllFieldAttributesInClass(typeof(TClass), typeof(TAttribute), bindingFlags);

        public static IEnumerable<FieldInfo> FindAllFieldAttributesInClass(Type classType, Type attributeType, BindingFlags bindingFlags = defaultFlags) =>
            classType.GetFields(bindingFlags)
                .Where(x => x.GetCustomAttributes(attributeType, false).Count() > 0);

        public static IEnumerable<PropertyInfo> FindAllPropertyAttributesInClass<TClass, TAttribute>(BindingFlags bindingFlags = defaultFlags)
            where TClass : class
            where TAttribute : Attribute =>
            typeof(TClass).GetProperties(bindingFlags)
                .Where(x => x.GetCustomAttributes<TAttribute>(false).Count() > 0);

        public static IEnumerable<PropertyInfo> FindAllPropertyAttributesInClass(Type classType, Type attributeType, BindingFlags bindingFlags = defaultFlags) =>
            classType.GetProperties(bindingFlags)
                .Where(x => x.GetCustomAttributes(attributeType, false).Count() > 0);

        public static List<FieldInfo> FindAllFieldAttributesInClassList<TClass, TAttribute>(BindingFlags bindingFlags = defaultFlags)
            where TClass : class
            where TAttribute : Attribute =>
            FindAllFieldAttributesInClassList(typeof(TClass), typeof(TAttribute), bindingFlags)
                .ToList();

        public static List<FieldInfo> FindAllFieldAttributesInClassList(Type classType, Type attributeType, BindingFlags bindingFlags = defaultFlags) =>
            FindAllFieldAttributesInClass(classType, attributeType, bindingFlags)
                .ToList();

        public static List<PropertyInfo> FindAllPropertyAttributesInClassList<TClass, TAttribute>()
            where TClass : class
            where TAttribute : Attribute =>
            FindAllPropertyAttributesInClassList(typeof(TClass), typeof(TAttribute))
                .ToList();

        public static List<PropertyInfo> FindAllPropertyAttributesInClassList(Type classType, Type attributeType) =>
            FindAllPropertyAttributesInClass(classType, attributeType)
                .ToList();

        public static object CreateConstructorFromType(Type type) =>
            CreateConstructorFromType(type, null);

        public static object CreateConstructorFromType(Type type, params object[] parameters)
        {
            if (type == null)
                return null;

            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null || constructor.IsAbstract) return null;
            return constructor.Invoke(parameters);
        }

        public static IEnumerable<T> CreateConstructorsFromTypes<T>(IEnumerable<Type> types) =>
            types.SelectMany(x =>
            {
                if (x == null)
                    return new T[] { default };

                ConstructorInfo constructor = x.GetConstructor(Type.EmptyTypes);
                if (constructor == null || constructor.IsAbstract) return new T[0];
                return new T[] { (T)constructor.Invoke(null) };
            });

        public static List<T> CreateConstructorsFromTypesList<T>(IEnumerable<Type> types) =>
            CreateConstructorsFromTypes<T>(types)
            .ToList();
    }
}