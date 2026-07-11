using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginContract;

namespace PluginRunner
{
    public class PluginMetadata
    {
        public Type PluginType { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<string> Dependencies { get; set; } = new();
    }

    public static class PluginEngine
    {
        public static List<PluginMetadata> DiscoverPlugins(string folderPath)
        {
            var plugins = new List<PluginMetadata>();
            if (!Directory.Exists(folderPath)) return plugins;

            var dlls = Directory.GetFiles(folderPath, "*.dll");
            foreach (var dll in dlls)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);
                    foreach (var type in assembly.GetTypes())
                    {
                        if (typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            var attr = type.GetCustomAttribute<PluginLoadAttribute>();
                            if (attr != null)
                            {
                                plugins.Add(new PluginMetadata
                                {
                                    PluginType = type,
                                    Name = attr.Name,
                                    Dependencies = attr.Dependencies.ToList()
                                });
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return plugins;
        }

        public static List<PluginMetadata> SortPlugins(List<PluginMetadata> plugins)
        {
            var sorted = new List<PluginMetadata>();
            var visited = new Dictionary<string, bool>();
            var pluginMap = plugins.ToDictionary(p => p.Name, p => p);

            foreach (var plugin in plugins)
            {
                Visit(plugin.Name, pluginMap, visited, sorted);
            }

            return sorted;
        }

        private static void Visit(string name, Dictionary<string, PluginMetadata> pluginMap, Dictionary<string, bool> visited, List<PluginMetadata> sorted)
        {
            if (visited.TryGetValue(name, out bool inProcess))
            {
                if (inProcess) throw new InvalidOperationException("Circular dependency detected");
                return;
            }

            if (!pluginMap.TryGetValue(name, out var plugin)) return;

            visited[name] = true;
            foreach (var dep in plugin.Dependencies)
            {
                Visit(dep, pluginMap, visited, sorted);
            }
            visited[name] = false;

            if (!sorted.Any(s => s.Name == name))
            {
                sorted.Add(plugin);
            }
        }

        public static void RunPlugins(List<PluginMetadata> sortedPlugins)
        {
            foreach (var plugin in sortedPlugins)
            {
                var instance = (ICommand)Activator.CreateInstance(plugin.PluginType)!;
                instance.Execute();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string path = args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                var plugins = PluginEngine.DiscoverPlugins(path);
                var sorted = PluginEngine.SortPlugins(plugins);
                PluginEngine.RunPlugins(sorted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
