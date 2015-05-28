using System;
using DynamicConfig.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicConfig.Tests
{
    [TestClass]
    public class JsonConfigProviderTests
    {
        [TestMethod]
        public void ParseTest()
        {
            var json = "{name:\"John\", surname:\"Doe\"}";

            var provider = new JsonConfigProvider();
            dynamic config = provider.Parse("default", json);

            Assert.IsNotNull(config);
            Assert.AreEqual("John", config.name);
            Assert.AreEqual("Doe", config.surname);
        }

        [TestMethod]
        public void ParseComplexTest()
        {
            var json = "{name:\"John\", complex:{ one: 1, two: 2, three: \"three\" } }";

            var provider = new JsonConfigProvider();
            dynamic config = provider.Parse("default", json);

            Assert.IsNotNull(config);
            Assert.AreEqual("John", config.name);
            Assert.IsNotNull(config.complex);
            Assert.AreEqual(1, (int)config.complex.one);
            Assert.AreEqual(2, (int)config.complex.two);
            Assert.AreEqual("three", (string)config.complex.three);
        }

        [TestMethod]
        public void LoadTest()
        {
            var filename = "simple.json";

            var provider = new JsonConfigProvider();
            dynamic config = provider.Load("default", filename);

            Assert.IsNotNull(config);
            Assert.AreEqual("John", config.name);
            Assert.AreEqual("Doe", config.surname);
        }

        [TestMethod]
        public void LoadComplexTest()
        {
            var filename = "complex.json";

            var provider = new JsonConfigProvider();
            dynamic config = provider.Load("default", filename);

            Assert.IsNotNull(config);
            Assert.AreEqual("John", config.name);
            Assert.AreEqual("Doe", config.surname);
            Assert.IsNotNull(config.complex);
            Assert.AreEqual(1, (int)config.complex.one);
            Assert.AreEqual(2, (int)config.complex.two);
            Assert.AreEqual("three", (string)config.complex.three);
        }

        [TestMethod]
        public void GetValueTest()
        {
            var json = "{name:\"John\", surname:\"Doe\"}";

            dynamic provider = new JsonConfigProvider();
            provider.Parse("Default", json);

            Assert.IsNotNull(provider.Default);
            Assert.AreEqual("John", provider.Default.name);
            Assert.AreEqual("Doe", provider.Default.surname);
        }
    }
}
