using System;
using System.Collections.Generic;
using System.Dynamic;

namespace DynamicConfig.Models
{
    public class ConfigObject : DynamicObject, IDictionary<string, object>
    {
        internal Dictionary<string, object> members = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (members.ContainsKey(binder.Name))
                result = members[binder.Name];
            else
                result = new NullExceptionPreventer();

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (this.members.ContainsKey(binder.Name))
                this.members[binder.Name] = value;
            else
                this.members.Add(binder.Name, value);
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
            members.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return members.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return members.Remove(key);
        }

        public object this[string key]
        {
            get
            {
                return members[key];
            }
            set
            {
                members[key] = value;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return members.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return members.Values;
            }
        }
        public bool TryGetValue(string key, out object value)
        {
            return members.TryGetValue(key, out value);
        }

        #endregion IDictionary implementation

        #region IEnumerable implementation

        public System.Collections.IEnumerator GetEnumerator()
        {
            return members.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return members.GetEnumerator();
        }

        #endregion IEnumerable implementation

        #region ICollection implementation

        public void Add(KeyValuePair<string, object> item)
        {
            members.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            members.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return members.ContainsKey(item.Key) && members[item.Key] == item.Value;
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
                return members.Count;
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
