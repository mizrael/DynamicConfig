using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicConfig.Tests
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void InvalidProviderName()
        {
            var provider = Config.GetProvider("lorem");
        }

        [TestMethod]
        public void ProviderRegistration()
        {
            var provider = new Providers.JsonConfigProvider();
            Config.RegisterProvider("json", provider);

            var registeredProvider = Config.GetProvider("json");

            Assert.IsNotNull(registeredProvider);
            Assert.AreSame(provider, registeredProvider);
        }

        [TestMethod]
        public void LoadTest()
        {
            var provider = new Providers.JsonConfigProvider();
            Config.RegisterProvider("json", provider);

            var filename = "simple.json";
                        
            dynamic config = Config.Load("json","default", filename);

            Assert.IsNotNull(config);
            Assert.AreEqual("John", config.name);
            Assert.AreEqual("Doe", config.surname);
        }

        [TestMethod]
        public void ParseTest()
        {
            var provider = new Providers.JsonConfigProvider();
            Config.RegisterProvider("json", provider);

            var json = "{name:\"John\", surname:\"Doe\"}";

            dynamic config = Config.Parse("json", "default", json);

            Assert.IsNotNull(config);
            Assert.AreEqual("John", config.name);
            Assert.AreEqual("Doe", config.surname);            
        }

        [TestMethod]
        public void SameConfig()
        {
            var providerName = "json";
            var provider = new Providers.JsonConfigProvider();
            Config.RegisterProvider(providerName, provider);

            var json = "{name:\"John\", surname:\"Doe\"}";
            dynamic config = Config.Parse(providerName, "config", json);

            var registeredProvider = Config.GetProvider(providerName);
            Assert.AreSame(registeredProvider.config, config);
        }
    }
}
