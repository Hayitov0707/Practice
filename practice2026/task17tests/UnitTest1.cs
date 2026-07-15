using Xunit;
using System;
using System.Threading;
using task17;

namespace task17tests
{
    public class SampleCommand : ICommand
    {
        public bool Executed { get; private set; }
        public void Execute() => Executed = true;
    }

    public class ServerThreadTests
    {
        [Fact]
        public void HardStop_ShouldStopImmediatelyIgnoringQueue()
        {
            var server = new ServerThread();
            var cmd1 = new SampleCommand();
            var stopCmd = new HardStopCommand(server);
            var cmd2 = new SampleCommand();

            server.Add(cmd1);
            server.Add(stopCmd);
            server.Add(cmd2);

            server.Start();
            server.UnderlyingThread.Join(2000);

            Assert.True(cmd1.Executed);
            Assert.False(cmd2.Executed);
        }

        [Fact]
        public void SoftStop_ShouldProcessAllRemainingCommands()
        {
            var server = new ServerThread();
            var cmd1 = new SampleCommand();
            var stopCmd = new SoftStopCommand(server);
            var cmd2 = new SampleCommand();

            server.Add(cmd1);
            server.Add(stopCmd);
            server.Add(cmd2);

            server.Start();
            server.UnderlyingThread.Join(2000);

            Assert.True(cmd1.Executed);
            Assert.True(cmd2.Executed);
        }

        [Fact]
        public void StopCommands_ShouldThrowExceptionOutsideServerThread()
        {
            var server = new ServerThread();
            var hardStop = new HardStopCommand(server);
            var softStop = new SoftStopCommand(server);

            Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
            Assert.Throws<InvalidOperationException>(() => softStop.Execute());
        }

        [Fact]
        public void ServerThread_ShouldAllowDynamicStrategyChange()
        {
            var server = new ServerThread();
            var cmd = new SampleCommand();
            bool strategyChanged = false;

            server.Strategy = c => { strategyChanged = true; c.Execute(); };
            server.Add(cmd);
            server.Add(new HardStopCommand(server));

            server.Start();
            server.UnderlyingThread.Join(2000);

            Assert.True(strategyChanged);
            Assert.True(cmd.Executed);
        }
    }
}
