using System.Collections;
using System.Linq;
using DynamicConfig.Models;
using DynamicConfig.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicConfig.Tests
{
    [TestClass]
    public class JsonConfigProviderTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            var currDir = System.IO.Directory.GetCurrentDirectory();
            var testFiles = System.IO.Directory.GetFiles(currDir, "test*.json");
            foreach(var file in testFiles)
                System.IO.File.Delete(file);
        }

        #region parse

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
            Assert.AreEqual(1, config.complex.one);
            Assert.AreEqual(2, config.complex.two);
            Assert.AreEqual("three", config.complex.three);
        }

        [TestMethod]
        public void ParseComplexArrayTest()
        {
            var json = "{name:\"John\", complex : { one: 1, myArray: [ 2, 3, 4 ] } }";

            var provider = new JsonConfigProvider();
            dynamic config = provider.Parse("default", json);

            Assert.IsNotNull(config);            
            Assert.IsNotNull(config.complex);
            Assert.AreEqual(1, config.complex.one);
            Assert.IsNotNull(config.complex.myArray);
            Assert.IsTrue(config.complex.myArray is IEnumerable);
        }

        [TestMethod]
        public void ParseInnerArrayTest()
        {
            var json = "{ myArray: [ 2, 3, 4 ] }";

            var provider = new JsonConfigProvider();
            dynamic config = provider.Parse("default", json);

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.myArray);

            var myArray = config.myArray as IEnumerable;
            Assert.IsNotNull(myArray);
            Assert.AreEqual(3, myArray.Cast<object>().Count());
        }

        [TestMethod]
        public void ParseInnerMixedArrayTest()
        {
            var json = "{ myArray: [ 2, 3, \"John\" ] }";

            var provider = new JsonConfigProvider();
            dynamic config = provider.Parse("default", json);

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.myArray);

            var myArray = config.myArray as IEnumerable;
            Assert.IsNotNull(myArray);

            var myArrayConfig = myArray as ConfigObject;
            Assert.IsNotNull(myArrayConfig); 
            Assert.AreEqual(3, myArrayConfig.Count);

            Assert.AreEqual("John", myArrayConfig[2]);
        }

        [TestMethod]
        public void ParseInnerMixedComplexArrayTest()
        {
            var json = "{ myArray: [ 2, \"John\", { myProp1: 1, myProp2: \"Doe\" } ] }";

            var provider = new JsonConfigProvider();
            dynamic config = provider.Parse("default", json);

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.myArray);

            var myArray = config.myArray as IEnumerable;
            Assert.IsNotNull(myArray);

            var myArrayConfig = myArray as ConfigObject;
            Assert.IsNotNull(myArrayConfig); 
            Assert.AreEqual(3, myArrayConfig.Count);

            var innerObject = myArrayConfig[2];
            Assert.AreEqual(1, innerObject.myProp1);
            Assert.AreEqual("Doe", innerObject.myProp2);
        }

        #endregion parse

        #region load

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
            Assert.AreEqual(1, config.complex.one);
            Assert.AreEqual(2, config.complex.two);
            Assert.AreEqual("three", config.complex.three);
        }

        #endregion load

        #region get

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
        
        [TestMethod]
        public void GetNonExistentValueTest()
        {
            dynamic provider = new JsonConfigProvider();

            var notExistent = provider.notExistent;

            Assert.IsNotNull(notExistent);
            Assert.IsFalse(notExistent);
        }

        #endregion get

        #region set

        [TestMethod]
        public void SetValueTest()
        {
            var json = "{name:\"John\", surname:\"Doe\"}";

            dynamic provider = new JsonConfigProvider();
            provider.Parse("Default", json);
                        
            Assert.AreEqual("John", provider.Default.name);

            provider.Default.name = "Bill";
            Assert.AreEqual("Bill", provider.Default.name);
        }

        [TestMethod]
        public void SetComplexValueTest()
        {
            var json = "{name:\"John\", complex:{ one: 1, two: 2, three: \"three\" } }";

            dynamic provider = new JsonConfigProvider();
            provider.Parse("Default", json);

            Assert.AreEqual(1, provider.Default.complex.one);

            provider.Default.complex.one = "one";
            Assert.AreEqual("one", provider.Default.complex.one);
        }

        [TestMethod]
        public void SetToComplexValueTest()
        {
            var json = "{name:\"John\", complex:{ one: 1, two: 2, three: \"three\" } }";

            dynamic provider = new JsonConfigProvider();
            provider.Parse("Default", json);
            
            provider.Default.otherComplex = new[] { "my", "second", "complex" };
            Assert.IsNotNull(provider.Default.otherComplex);
        }

        #endregion set

        #region save

        [TestMethod]
        public void SaveTest()
        {
            var configFile = "simple.json";
            var filename = CreateTempConfigFile(configFile);

            var provider = new JsonConfigProvider();
            dynamic config = provider.Load("default", filename);
                        
            Assert.AreEqual("John", config.name);

            config.name = "Bill";
            Assert.AreEqual("Bill", config.name);

            dynamic config2 = provider.Load("loadedDefault", filename);
            Assert.AreEqual("Bill", config2.name);
        }

        [TestMethod]
        public void SaveComplexTest()
        {
            var configFile = "complex.json";
            var filename = CreateTempConfigFile(configFile);

            var provider = new JsonConfigProvider();
            dynamic config = provider.Load("default", filename);

            config.complex.one = "one";

            dynamic config2 = provider.Load("loadedDefault", filename);
            Assert.AreEqual("one", config2.complex.one);
        }

        #endregion save

        private static string CreateTempConfigFile(string configFile)
        {
            var filename = "test" + System.DateTime.UtcNow.Ticks.ToString() + ".json";
            System.IO.File.Copy(configFile, filename);
            return filename;
        }
    }
}
