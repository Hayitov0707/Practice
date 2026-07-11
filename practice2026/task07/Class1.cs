using System;
using System.Reflection;
using System.Linq;

namespace task07
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }
        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class VersionAttribute : Attribute
    {
        public int Major { get; }
        public int Minor { get; }
        public VersionAttribute(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }
    }

    [DisplayName("Пример класса")]
    [Version(1, 0)]
    public class SampleClass
    {
        [DisplayName("Числовое свойство")]
        public int Number { get; set; }

        [DisplayName("Тестовый метод")]
        public void TestMethod() { }
    }

    public static class ReflectionHelper
    {
        public static void PrintTypeInfo(Type type)
        {
            if (type == null) return;

            Console.WriteLine($"--- Анализ типа: {type.Name} ---");

            var typeDisplay = type.GetCustomAttribute<DisplayNameAttribute>();
            if (typeDisplay != null)
            {
                Console.WriteLine($"Отображаемое имя класса: {typeDisplay.DisplayName}");
            }

            var typeVersion = type.GetCustomAttribute<VersionAttribute>();
            if (typeVersion != null)
            {
                Console.WriteLine($"Версия класса: {typeVersion.Major}.{typeVersion.Minor}");
            }

            Console.WriteLine("Свойства:");
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (var prop in properties)
            {
                var propDisplay = prop.GetCustomAttribute<DisplayNameAttribute>();
                if (propDisplay != null)
                {
                    Console.WriteLine($"  - {prop.Name}: {propDisplay.DisplayName}");
                }
            }

            Console.WriteLine("Методы:");
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                var methodDisplay = method.GetCustomAttribute<DisplayNameAttribute>();
                if (methodDisplay != null)
                {
                    Console.WriteLine($"  - {method.Name}(): {methodDisplay.DisplayName}");
                }
            }
        }
    }
}
