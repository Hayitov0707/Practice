using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task18
{
    public interface ICommand
    {
        void Execute();
    }

    public interface IScheduler
    {
        bool HasCommand();
        ICommand? Select();
        void Add(ICommand cmd);
    }

    public class RoundRobinScheduler : IScheduler
    {
        private readonly ConcurrentQueue<ICommand> _commands = new();

        public bool HasCommand() => !_commands.IsEmpty;

        public ICommand? Select()
        {
            return _commands.TryDequeue(out var cmd) ? cmd : null;
        }

        public void Add(ICommand cmd)
        {
            if (cmd != null)
            {
                _commands.Enqueue(cmd);
            }
        }
    }

    public class SchedulerServerThread
    {
        private readonly BlockingCollection<ICommand> _queue = new();
        private readonly IScheduler _scheduler;
        private readonly Thread _thread;
        private bool _stopRequested;

        public Action<ICommand> Strategy { get; set; }
        public Thread UnderlyingThread => _thread;

        public SchedulerServerThread(IScheduler scheduler)
        {
            _scheduler = scheduler;
            Strategy = cmd => cmd.Execute();
            _thread = new Thread(Run);
        }

        public void Start() => _thread.Start();

        public void Add(ICommand command) => _queue.Add(command);

        public void Stop()
        {
            _stopRequested = true;
            _queue.CompleteAdding();
        }

        private void Run()
        {
            while (!_stopRequested)
            {
                try
                {
                    if (_scheduler.HasCommand())
                    {
                        if (_queue.TryTake(out var newCommand))
                        {
                            Strategy(newCommand);
                        }

                        ICommand? scheduledCmd = _scheduler.Select();
                        if (scheduledCmd != null)
                        {
                            Strategy(scheduledCmd);
                        }
                    }
                    else
                    {
                        ICommand newCommand = _queue.Take();
                        Strategy(newCommand);
                    }
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
