using System;
using System.Collections.Generic;

namespace TSQLLint.Common.Tests
{
    public class TestPlugin : IPlugin
    {
        public void PerformAction(IPluginContext context, IReporter reporter)
        {
            throw new NotImplementedException();
        }
    }

    // TODO: Remove this class when removing IPluginWithRules
    public class TestPluginWithRules : IPluginWithRules
    {
        public void PerformAction(IPluginContext context, IReporter reporter) => throw new NotImplementedException();

        public Dictionary<string, ISqlLintRule> Rules => throw new NotImplementedException();
    }
}
