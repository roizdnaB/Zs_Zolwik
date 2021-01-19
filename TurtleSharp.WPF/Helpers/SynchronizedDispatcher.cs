using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TurtleSharp.WPF.Helpers
{
    public class SynchronizedDispatcher
    {
        private Dispatcher _dispatcher;
        private Queue<Action> _queue = new Queue<Action>();

        public event EventHandler QueueEmptied;

        private object _syncRoot = new object();


        private CancellationToken _cancellationToken;
        public bool Waiting { get; private set; }
        public bool IsEmpty => _queue.Count == 0;
        public CancellationToken CancellationToken { get { return _cancellationToken; } set {
                _cancellationToken = value;
                _cancellationToken.Register(() => {
                    _queue.Clear();
                    Waiting = false;
                    }
                );
            }
        }
        public SynchronizedDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Invoke(Action action)
        {
            CheckCancellationRequested();
            if (CancellationToken.IsCancellationRequested) return;
            Action dispatcherAction = () =>
            {
                var operation = _dispatcher.InvokeAsync(action, DispatcherPriority.Normal, _cancellationToken);
                operation.Completed += delegate
                {
                    TryDispatchNext();
                };
            };

            lock (_syncRoot) {
                _queue.Enqueue(dispatcherAction);
                if (dispatcherAction == _queue.Peek() && !Waiting) TryDispatchNext();
            }
        }

        public void Invoke(Action<ActionCompletedNotifier> action)
        {
            CheckCancellationRequested();
            if (CancellationToken.IsCancellationRequested) return;

            Action innerAction = () =>
            {
                var afn = new ActionCompletedNotifier();
                afn.Completed += delegate { TryDispatchNext(); };
                action.Invoke(afn);
            };

            Action dispatcherInvocation = () =>
                _dispatcher.InvokeAsync(innerAction, DispatcherPriority.Normal, _cancellationToken);

            lock (_syncRoot)
            {
                _queue.Enqueue(dispatcherInvocation);
                if (dispatcherInvocation == _queue.Peek() && !Waiting) TryDispatchNext();
            }
        }

        public class ActionCompletedNotifier
        {
            public event EventHandler Completed;

            public void NotifyCompletion()
            {
                Completed.Invoke(this, null);
            }
        }
        
        private void TryDispatchNext()
        {
            CheckCancellationRequested();

            if (_queue.Count > 0)
            {
                Waiting = true;
                _queue.Dequeue().Invoke();
            }
            else
            {
                Waiting = false;
                QueueEmptied.Invoke(this, null);
            }
        }

        private void CheckCancellationRequested()
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                _queue.Clear();
            }
        }
    }
}
