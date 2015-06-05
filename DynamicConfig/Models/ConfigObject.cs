using System;
using System.Collections.Generic;
using System.Dynamic;

namespace DynamicConfig.Models
{
    public class ConfigObject : DynamicObject, IDictionary<string, object>
    {
        internal Dictionary<string, object> _members = null;

        #region cTors

        public ConfigObject()
        {
            _members = new Dictionary<string, object>();
        }

        public ConfigObject(IDictionary<string, object> dictionary)
        {
            if (null == dictionary) throw new ArgumentNullException("values");
            _members = new Dictionary<string, object>(dictionary);
        }

        #endregion cTors

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_members.ContainsKey(binder.Name))
                result = _members[binder.Name];
            else
                result = new NullExceptionPreventer();

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (this._members.ContainsKey(binder.Name))
                this._members[binder.Name] = value;
            else
                this._members.Add(binder.Name, value);
            return true;
        }

        public static ConfigObject FromExpando(ExpandoObject e)
        {
            var edict = e as IDictionary<string, object>;
            var c = new ConfigObject();
            var cdict = (IDictionary<string, object>)c;

            // won't work for generic ExpandoObjects which might include collections etc.
            foreach (var kvp in edict)
            {
                // recursively convert and add ExpandoObjects
                if (kvp.Value is ExpandoObject)
                {
                    cdict.Add(kvp.Key, FromExpando((ExpandoObject)kvp.Value));
                }
                else if (kvp.Value is ExpandoObject[])
                {
                    var config_objects = new List<ConfigObject>();
                    foreach (var ex in ((ExpandoObject[])kvp.Value))
                    {
                        config_objects.Add(FromExpando(ex));
                    }
                    cdict.Add(kvp.Key, config_objects.ToArray());
                }
                else
                    cdict.Add(kvp.Key, kvp.Value);
            }
            return c;
        }

        #region IDictionary implementation

        public void Add(string key, object value)
        {
            _members.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _members.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _members.Remove(key);
        }

        public object this[string key]
        {
            get
            {
                return _members[key];
            }
            set
            {
                _members[key] = value;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return _members.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return _members.Values;
            }
        }
        public bool TryGetValue(string key, out object value)
        {
            return _members.TryGetValue(key, out value);
        }

        #endregion IDictionary implementation

        #region IEnumerable implementation

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        #endregion IEnumerable implementation

        #region ICollection implementation

        public void Add(KeyValuePair<string, object> item)
        {
            _members.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _members.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _members.ContainsKey(item.Key) && _members[item.Key] == item.Value;
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new System.NotImplementedException();
        }

        public int Count
        {
            get
            {
                return _members.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion ICollection implementation
    }
}
