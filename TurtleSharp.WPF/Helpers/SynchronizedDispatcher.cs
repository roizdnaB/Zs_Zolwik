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

        private object _syncRoot = new object();

        public bool Waiting { get; private set; }
        public CancellationToken? CancellationToken { get; set; }
        public SynchronizedDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Invoke(Action action)
        {
            Action dispatcherAction = () =>
            {
                var operation = (CancellationToken is null) ? _dispatcher.InvokeAsync(action, DispatcherPriority.Normal)
                : _dispatcher.InvokeAsync(action, DispatcherPriority.Normal, (CancellationToken)CancellationToken);
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
            Action innerAction = () =>
            {
                var afn = new ActionCompletedNotifier();
                afn.Completed += delegate { TryDispatchNext(); };
                action.Invoke(afn);
            };

            Action dispatcherInvocation = () =>
            {
                if (CancellationToken is null) _dispatcher.InvokeAsync(innerAction, DispatcherPriority.Normal);
                else _dispatcher.InvokeAsync(innerAction, DispatcherPriority.Normal, (CancellationToken)CancellationToken);
            };

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
            if (_queue.Count > 0)
            {
                Waiting = true;
                _queue.Dequeue().Invoke();
            }
            else
            {
                Waiting = false;
            }
        }
    }
}
