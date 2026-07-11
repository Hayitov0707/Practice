using Xunit;
using System;
using System.Collections.Generic;
using PluginRunner;

namespace task10tests
{
    public class PluginSystemTests
    {
        [Fact]
        public void SortPlugins_ShouldOrderCorrectly()
        {
            var plugins = new List<PluginMetadata>
            {
                new PluginMetadata { Name = "PluginB", Dependencies = new List<string> { "PluginA" } },
                new PluginMetadata { Name = "PluginA", Dependencies = new List<string>() }
            };

            var sorted = PluginEngine.SortPlugins(plugins);

            Assert.Equal(2, sorted.Count);
            Assert.Equal("PluginA", sorted[0].Name);
            Assert.Equal("PluginB", sorted[1].Name);
        }

        [Fact]
        public void SortPlugins_ShouldThrowOnCircularDependency()
        {
            var plugins = new List<PluginMetadata>
            {
                new PluginMetadata { Name = "PluginX", Dependencies = new List<string> { "PluginY" } },
                new PluginMetadata { Name = "PluginY", Dependencies = new List<string> { "PluginX" } }
            };

            Assert.Throws<InvalidOperationException>(() => PluginEngine.SortPlugins(plugins));
        }
    }
}
