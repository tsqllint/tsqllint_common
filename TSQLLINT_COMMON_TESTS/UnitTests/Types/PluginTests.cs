using NUnit.Framework;
using System;

namespace TSQLLINT_COMMON_TESTS
{
    public class PluginTests
    {
        [Test]
        public void TestPlugin()
        {
            var testPlugin = new TestPlugin();
            Assert.Throws<NotImplementedException>(() => { testPlugin.PerformAction(null); });
        }
    }
}
