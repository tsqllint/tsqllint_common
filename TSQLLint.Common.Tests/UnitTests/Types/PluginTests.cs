using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace TSQLLint.Common.Tests
{
    public class PluginTests
    {
        [Test]
        public void TestPlugin()
        {
            IPlugin testPlugin = new TestPlugin();
            Assert.Throws<NotImplementedException>(() => { testPlugin.PerformAction(null, null); });
            Assert.That(testPlugin.GetRules(), Is.Empty.And.InstanceOf<IDictionary<string, ISqlLintRule>>());
        }

        // TODO: Remove this test when removing IPluginWithRules
        [Test]
        public void TestPluginWithRulesAsIPlugin()
        {
            IPlugin testPluginWithRules = new TestPluginWithRules();
            Assert.Throws<NotImplementedException>(() => { testPluginWithRules.PerformAction(null, null); });
            Assert.Throws<NotImplementedException>(() => { _ = testPluginWithRules.GetRules(); });
        }
    }
}
