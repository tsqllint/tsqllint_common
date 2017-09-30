using NUnit.Framework;
using System;

namespace TSQLLint.Common.Tests
{
    public class PluginTests
    {
        [Test]
        public void TestPlugin()
        {
            var testPlugin = new TestPlugin();
            Assert.Throws<NotImplementedException>(() => { testPlugin.PerformAction(null, null); });
        }
    }
}
