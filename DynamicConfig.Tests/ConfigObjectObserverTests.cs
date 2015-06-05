using System;
using DynamicConfig.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicConfig.Tests
{
    [TestClass]
    public class ConfigObjectObserverTests : IObserver<ConfigObject>
    {
        private bool _set = false;

        [TestInitialize]
        private void Init()
        {
            _set = false;
        }

        [TestMethod]
        public void SetSimpleTest()
        {
            Assert.IsFalse(_set);

            var config = new ConfigObject();
            config.Subscribe(this);

            var dynConf = config as dynamic;
            dynConf.data = "lorem";

            Assert.IsTrue(_set);
        }

        [TestMethod]
        public void SetComplexTest()
        {
            Assert.IsFalse(_set);

            var config = new ConfigObject();
            config.Subscribe(this);

            var dynConf = config as dynamic;
            dynConf.data.complex = "lorem";

            Assert.IsTrue(_set);
        }

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
            _set = true;
        }
    }
}
