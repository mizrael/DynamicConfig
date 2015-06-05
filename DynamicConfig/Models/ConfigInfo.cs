using System;
using System.Collections.Generic;

namespace DynamicConfig.Models
{
    public class ConfigInfo : IObserver<ConfigObject>, IObservable<ConfigInfo>
    {
        private List<IObserver<ConfigInfo>> _observers;

        public ConfigInfo(string uniqueName)
        {
            if (string.IsNullOrWhiteSpace(uniqueName))
                throw new ArgumentNullException("uniqueName");
            this.UniqueName = uniqueName;

            _observers = new List<IObserver<ConfigInfo>>();
        }

        public string UniqueName { get; private set; }

        public string Filename { get; set; }        

        public ConfigObject Data { get; set; }

        #region IObserver

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(ConfigObject value)
        {
            foreach (var observer in _observers)
                observer.OnNext(this);
        }

        #endregion IObserver

        #region IObservable

        public IDisposable Subscribe(IObserver<ConfigInfo> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);

            return new Unsubscriber<ConfigInfo>(_observers, observer);
        }

        #endregion IObservable
    }
}
