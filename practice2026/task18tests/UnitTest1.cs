using Xunit;
using System;
using System.Collections.Generic;
using System.Threading;
using task18;

namespace task18tests
{
    public class LongRunningCommand : ICommand
    {
        private readonly string _id;
        private readonly int _totalSteps;
        private int _currentStep;
        private readonly IScheduler _scheduler;
        private readonly List<string> _executionLog;

        public LongRunningCommand(string id, int steps, IScheduler scheduler, List<string> log)
        {
            _id = id;
            _totalSteps = steps;
            _scheduler = scheduler;
            _executionLog = log;
        }

        public void Execute()
        {
            _currentStep++;
            lock (_executionLog)
            {
                _executionLog.Add($"{_id}-{_currentStep}");
            }

            if (_currentStep < _totalSteps)
            {
                _scheduler.Add(this);
            }
        }
    }

    public class SchedulerTests
    {
        [Fact]
        public void Scheduler_ShouldExecuteCommandsInRoundRobinOrder()
        {
            var scheduler = new RoundRobinScheduler();
            var log = new List<string>();

            var cmdA = new LongRunningCommand("A", 3, scheduler, log);
            var cmdB = new LongRunningCommand("B", 2, scheduler, log);

            var server = new SchedulerServerThread(scheduler);

            scheduler.Add(cmdA);
            scheduler.Add(cmdB);

            server.Start();

            Thread.Sleep(300);
            server.Stop();
            server.UnderlyingThread.Join(1000);

            Assert.Equal(5, log.Count);
            Assert.Equal("A-1", log[0]);
            Assert.Equal("B-1", log[1]);
            Assert.Equal("A-2", log[2]);
            Assert.Equal("B-2", log[3]);
            Assert.Equal("A-3", log[4]);
        }
    }
}
