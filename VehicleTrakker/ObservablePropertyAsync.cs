using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace VehicleTrakker
{
    public class ObservablePropertyAsync<T>
    {
        private readonly List<Func<T, Task>> observers;
        private readonly object observableLock = new object();

        public ObservablePropertyAsync()
        {
            observers = new List<Func<T, Task>>();
        }

        private class UnsubscriberAsync : IDisposable
        {
            private readonly List<Func<T, Task>> _observers;
            private readonly Func<T, Task> _observer;

            public UnsubscriberAsync(List<Func<T, Task>> observers, Func<T, Task> observer)
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

        public IDisposable Subscribe(Func<T, Task> action)
        {

            if (!observers.Contains(action))
            {
                lock (observableLock)
                {
                    observers.Add(action);
                }
            }
            return new UnsubscriberAsync(observers, action);
        }

        public async Task PublishAsync(T value)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
             async () =>
             {
                 foreach (var observer in observers.ToArray())
                 {
                     await observer(value);
                 }
             });
        }
    }
}
