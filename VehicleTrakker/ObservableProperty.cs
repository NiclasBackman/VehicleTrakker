using System;
using System.Collections.Generic;

namespace NavigationTest
{
    public class ObservableProperty<T>
    {
        private readonly List<Action<T>> observers;
        private readonly object observableLock = new object();

        public ObservableProperty()
        {
            observers = new List<Action<T>>();
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<Action<T>> _observers;
            private readonly Action<T> _observer;

            public Unsubscriber(List<Action<T>> observers, Action<T> observer)
            {
                this._observers = observers;
                this._observer = observer;

            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public IDisposable Subscribe(Action<T> action)
        {

            if (!observers.Contains(action))
            {
                lock(observableLock)
                {
                    observers.Add(action);
                }
            }
            return new Unsubscriber(observers, action);
        }

        public void Publish(T value)
        {
            lock (observableLock)
            {
                foreach (var observer in observers.ToArray())
                {
                    observer(value);
                }
            }
        }
    }
}
