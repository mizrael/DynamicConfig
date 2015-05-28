using System;
using DynamicConfig.Providers;
using System.Collections.Concurrent;

namespace DynamicConfig
{
    public static class Config
    {
        private static ConcurrentDictionary<string, IConfigProvider> _providers = null;

        static Config()
        {
            _providers = _providers ?? new ConcurrentDictionary<string, IConfigProvider>();
        }

        public static void RegisterProvider(string name, IConfigProvider provider)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (null == provider)
                throw new ArgumentNullException("provider");

            _providers.AddOrUpdate(name, provider, (n, p) => p);           
        }

        public static dynamic GetProvider(string name)
        {
             if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            IConfigProvider provider;
            if (!_providers.TryGetValue(name, out provider) || null == provider)
                throw new ArgumentException("invalid name: " + name);

            return provider as dynamic;
        }

        public static DynamicConfig.Models.ConfigObject Load(string providerName, string uniqueName, string fullPath) {
            if (string.IsNullOrWhiteSpace(uniqueName))
                throw new ArgumentNullException("uniqueName");
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentNullException("fullPath");

            var provider = Config.GetProvider(providerName) as IConfigProvider;
            if(null == provider)
                throw new ArgumentException("invalid name: " + providerName);

            return provider.Load(uniqueName, fullPath);        
        }

        public static DynamicConfig.Models.ConfigObject Parse(string providerName, string uniqueName, string json)
        {
            if (string.IsNullOrWhiteSpace(uniqueName))
                throw new ArgumentNullException("uniqueName");
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentNullException("json");

            var provider = Config.GetProvider(providerName) as IConfigProvider;
            if (null == provider)
                throw new ArgumentException("invalid name: " + providerName);

            return provider.Parse(uniqueName, json);
        }        
    }   

}
