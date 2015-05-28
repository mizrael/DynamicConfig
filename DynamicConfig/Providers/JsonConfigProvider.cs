using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using DynamicConfig.Models;
using Newtonsoft.Json;

namespace DynamicConfig.Providers
{
    public class JsonConfigProvider : DynamicObject, IConfigProvider
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
            return AddOrUpdate(uniqueName, json);
        }

        public ConfigObject Parse(string uniqueName, string json)
        {
            if (string.IsNullOrWhiteSpace(uniqueName))
                throw new ArgumentNullException("uniqueName");
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentNullException("json");

            return AddOrUpdate(uniqueName, json);
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

        private ConfigObject AddOrUpdate(string uniqueName, string json)
        {
            var parsedConfig = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);

            ConfigObject data = Merger.Merge(new ConfigObject(), parsedConfig);

            var info = new ConfigInfo(uniqueName)
            {
                Data = data
            };

            info = _configs.AddOrUpdate(uniqueName, info, (s, d) => { return info; });
            return info.Data;
        }

        #endregion Private Methods
    }

  
}
