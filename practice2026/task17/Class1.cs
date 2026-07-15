using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17
{
    public interface ICommand
    {
        void Execute();
    }

    public class ServerThread
    {
        private readonly BlockingCollection<ICommand> _queue = new();
        private readonly Thread _thread;
        private bool _stopRequested;

        public Action<ICommand> Strategy { get; set; }
        public Thread UnderlyingThread => _thread;

        public ServerThread()
        {
            Strategy = cmd => cmd.Execute();
            _thread = new Thread(Run);
        }

        public void Start() => _thread.Start();

        public void Add(ICommand command) => _queue.Add(command);

        public void HardStop()
        {
            if (Thread.CurrentThread != _thread)
            {
                throw new InvalidOperationException();
            }
            _stopRequested = true;
            _queue.CompleteAdding();
        }

        public void SoftStop()
        {
            if (Thread.CurrentThread != _thread)
            {
                throw new InvalidOperationException();
            }
            _queue.CompleteAdding();
        }

        private void Run()
        {
            while (!_stopRequested)
            {
                try
                {
                    ICommand command = _queue.Take();
                    try
                    {
                        Strategy(command);
                    }
                    catch (Exception)
                    {
                    }
                }
                catch (InvalidOperationException)
                {
                    break;
                }
            }
        }
    }

    public class HardStopCommand : ICommand
    {
        private readonly ServerThread _serverThread;

        public HardStopCommand(ServerThread serverThread)
        {
            _serverThread = serverThread;
        }

        public void Execute()
        {
            _serverThread.HardStop();
        }
    }

    public class SoftStopCommand : ICommand
    {
        private readonly ServerThread _serverThread;

        public SoftStopCommand(ServerThread serverThread)
        {
            _serverThread = serverThread;
        }

        public void Execute()
        {
            _serverThread.SoftStop();
        }
    }
}
