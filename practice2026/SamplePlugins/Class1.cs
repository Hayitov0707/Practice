using System;
using PluginContract;

namespace SamplePlugins
{
    [PluginLoad("PluginA")]
    public class PluginA : ICommand
    {
        public static bool Executed { get; set; }

        public void Execute()
        {
            Executed = true;
            Console.WriteLine("PluginA executed");
        }
    }

    [PluginLoad("PluginB", "PluginA")]
    public class PluginB : ICommand
    {
        public static bool Executed { get; set; }
        public static bool ExecutedAfterA { get; set; }

        public void Execute()
        {
            Executed = true;
            if (PluginA.Executed)
            {
                ExecutedAfterA = true;
            }
            Console.WriteLine("PluginB executed");
        }
    }
}
