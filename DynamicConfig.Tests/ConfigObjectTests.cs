using System;
using DynamicConfig.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicConfig.Tests
{
    [TestClass]
    public class ConfigObjectTests
    {
        [TestMethod]
        public void NullParentTest()
        {
            var config = new ConfigObject();
            Assert.IsNull(config.Parent);
        }

        [TestMethod]
        public void ParentTest()
        {
            var config = new ConfigObject();
            var innerConfig = new ConfigObject(config);

            Assert.IsNotNull(innerConfig.Parent);
            Assert.AreSame(config, innerConfig.Parent);
        }
    }
}
