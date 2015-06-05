using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using DynamicConfig.Models;
using Newtonsoft.Json;

namespace DynamicConfig.Providers
{
    public class JsonConfigProvider : DynamicObject, IConfigProvider, IObserver<ConfigObject>
    {
        private ConcurrentDictionary<string, ConfigInfo> _configs = null;

        public JsonConfigProvider()
        {
            _configs = new ConcurrentDictionary<string, ConfigInfo>();
        }

        public ConfigObject Load(string uniqueName, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(uniqueName))
                throw new ArgumentNullException("uniqueName");
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentNullException("fullPath");

            var json = System.IO.File.ReadAllText(fullPath);
            return AddOrUpdate(uniqueName, json, fullPath);
        }

        public ConfigObject Parse(string uniqueName, string json)
        {
            if (string.IsNullOrWhiteSpace(uniqueName))
                throw new ArgumentNullException("uniqueName");
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentNullException("json");

            return AddOrUpdate(uniqueName, json, string.Empty);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            ConfigInfo info;
            if(_configs.TryGetValue(binder.Name, out info) && null != info && null != info.Data)
                result = info.Data;
            else
                result = new NullExceptionPreventer();

            return true;
        }

        #region Private Methods

        private ConfigObject AddOrUpdate(string uniqueName, string json, string filename)
        {
            var parsedConfig = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);

            var data = new ConfigObject(null, parsedConfig);

            var info = new ConfigInfo(uniqueName)
            {
                Data = data,
                Filename = filename
            };

            info = _configs.AddOrUpdate(uniqueName, info, (s, d) => { return info; });                    
            info.Data.Subscribe(this);            

            return info.Data;
        }

        #endregion Private Methods

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
            return;
        }

        #endregion IObserver
    }  
}
