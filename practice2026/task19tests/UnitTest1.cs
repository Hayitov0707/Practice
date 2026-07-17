using Xunit;
using System;
using System.Threading;
using task19;

namespace task19tests
{
    public class LongRunningOperationsTests
    {
        [Fact]
        public void TestCommand_ShouldExecuteFiveInstancesThreeTimesAndHardStop()
        {
            var scheduler = new RoundRobinScheduler();
            var server = new ServerThread(scheduler);
            int totalCompleted = 0;

            server.Strategy = cmd =>
            {
                if (cmd is TestCommand tc)
                {
                    tc.Execute();
                    if (tc.counter < 3)
                    {
                        scheduler.Add(tc);
                    }
                    else
                    {
                        Interlocked.Increment(ref totalCompleted);
                        if (totalCompleted == 5)
                        {
                            server.Add(new HardStopCommand(server));
                        }
                    }
                }
                else
                {
                    cmd.Execute();
                }
            };

            for (int i = 1; i <= 5; i++)
            {
                scheduler.Add(new TestCommand(i));
            }

            server.Start();
            bool finished = server.UnderlyingThread.Join(4000);

            Assert.True(finished);
            Assert.Equal(5, totalCompleted);
        }
    }
}
