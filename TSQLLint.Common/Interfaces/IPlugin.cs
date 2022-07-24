using System.Collections.Generic;

namespace TSQLLint.Common
{
    public interface IPlugin
    {
        void PerformAction(IPluginContext context, IReporter reporter);

        IDictionary<string, ISqlLintRule> GetRules() => new Dictionary<string, ISqlLintRule>();
    }
}
