using System;

namespace PluginContract
{
    public interface ICommand
    {
        void Execute();
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PluginLoadAttribute : Attribute
    {
        public string Name { get; }
        public string[] Dependencies { get; }

        public PluginLoadAttribute(string name, params string[] dependencies)
        {
            Name = name;
            Dependencies = dependencies;
        }
    }
}
