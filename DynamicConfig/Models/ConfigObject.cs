using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicConfig.Models
{
    [JsonConverter(typeof(Serializers.ConfigObjectJsonSerializer))]
    public class ConfigObject : DynamicObject, IEnumerable, IObservable<ConfigObject>
    {
        #region members

        private Dictionary<string, object> _members = null;
        private List<IObserver<ConfigObject>> _observers;

        #endregion members

        #region cTors

        public ConfigObject() : this(null)
        {            
        }

        public ConfigObject(ConfigObject parent)
            : this(parent, null)
        {         
        }

        public ConfigObject(ConfigObject parent, IDictionary<string, object> dictionary)
        {
            this.Parent = parent;

            _observers = new List<IObserver<ConfigObject>>();
            _members = new Dictionary<string, object>();

            if (null != dictionary && dictionary.Any())
                this.AddRange(dictionary);
        }

        #endregion cTors

        #region public methods

        public void AddRange(IDictionary<string, object> dictionary)
        {
            if (null == dictionary)
                throw new ArgumentNullException("dictionary");

            foreach (var kvp in dictionary)
                _members[kvp.Key] = ProcessProperties(this, dictionary[kvp.Key]);
        }

        #endregion public methods

        #region DynamicObject

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_members.ContainsKey(binder.Name))
                result = _members[binder.Name];
            else
                result = new ConfigObject(this);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            value = ProcessProperties(this, value as dynamic);

            if (_members.ContainsKey(binder.Name))
                _members[binder.Name] = value;
            else
                _members.Add(binder.Name, value);

            Notify(this); 

            return true;
        }

        #endregion DynamicObject

        #region IEnumerable implementation

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        #endregion IEnumerable implementation

        #region IObservable

        protected void Notify(ConfigObject obj)
        {
            foreach (var observer in _observers)            
                observer.OnNext(obj);

            if (null != this.Parent)
                this.Parent.Notify(obj);
        }

        public IDisposable Subscribe(IObserver<ConfigObject> observer)
        {
            if (!_observers.Contains(observer))            
                _observers.Add(observer);

            return new Unsubscriber<ConfigObject>(_observers, observer);
        }       

        #endregion IObservable

        #region Properties

        public ConfigObject Parent { get; private set; }

        public IEnumerable<string> Keys
        {
            get
            {
                return _members.Keys;
            }
        }

        public dynamic this[string key]
        {
            get
            {
                if (_members.ContainsKey(key))
                    return _members[key];
                return new ConfigObject(this);
            }
            set
            {
                var processedValue = ProcessProperties(this, value as dynamic);
                _members[key] = processedValue;
            }
        }

        public dynamic this[int index]
        {
            get
            {
                var key = index.ToString();
                return this[key];
            }
            set
            {
                var processedValue = ProcessProperties(this, value as dynamic);
                _members[index.ToString()] = processedValue;
            }
        }

        public int Count
        {
            get
            {
                return _members.Count;
            }
        }
        
        #endregion Properties

        #region private methods

        private static dynamic ProcessProperties(ConfigObject parent, dynamic value)
        {
            if (value is string || value is ConfigObject)
                return value;

            var jObj = value as JObject;
            if (null != jObj)
            {
                var retVal = new ConfigObject(parent);

                var dict = new Dictionary<string, object>(jObj.Count);
                foreach (var kvp in jObj)
                    dict[kvp.Key] = ProcessProperties(retVal, jObj[kvp.Key]);

                retVal.AddRange(dict);
                
                return retVal;
            }

            var jVal = value as JValue;
            if (null != jVal)
                return jVal.Value;

            if (value is IEnumerable)
            {
                var retVal = new ConfigObject(parent);

                var tmpArray = new ArrayList();
                tmpArray.AddRange(value);
                for (int i = 0; i != tmpArray.Count; ++i)
                    retVal[i] = ProcessProperties(parent, tmpArray[i]);

                return retVal;
            }

            return value;
        }

        #endregion private methods
    }
}
