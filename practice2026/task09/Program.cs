using System;
using System.IO;
using System.Reflection;

namespace task09
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Error: Please provide a path to the DLL file.");
                return;
            }

            string dllPath = args[0];
            if (!File.Exists(dllPath))
            {
                Console.WriteLine("Error: File not found.");
                return;
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                Console.WriteLine($"Assembly: {assembly.FullName}");

                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsClass) continue;

                    Console.WriteLine($"\nClass: {type.FullName}");

                    Console.WriteLine("  Attributes:");
                    foreach (var attr in type.GetCustomAttributes())
                    {
                        Console.WriteLine($"    - {attr.GetType().Name}");
                    }

                    Console.WriteLine("  Constructors:");
                    foreach (var ctor in type.GetConstructors())
                    {
                        Console.WriteLine($"    - {ctor.Name}");
                        Console.WriteLine("      Parameters:");
                        foreach (var param in ctor.GetParameters())
                        {
                            Console.WriteLine($"        * {param.Name} : {param.ParameterType.Name}");
                        }
                    }

                    Console.WriteLine("  Methods:");
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    foreach (var method in methods)
                    {
                        Console.WriteLine($"    - {method.Name}");
                        Console.WriteLine("      Attributes:");
                        foreach (var attr in method.GetCustomAttributes())
                        {
                            Console.WriteLine($"        * {attr.GetType().Name}");
                        }
                        Console.WriteLine("      Parameters:");
                        foreach (var param in method.GetParameters())
                        {
                            Console.WriteLine($"        * {param.Name} : {param.ParameterType.Name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading assembly: {ex.Message}");
            }
        }
    }
}
