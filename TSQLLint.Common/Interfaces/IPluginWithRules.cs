using System.Collections.Generic;

namespace TSQLLint.Common
{
    public interface IPluginWithRules : IPlugin
    {
        /// <summary>
        /// Required if you
        /// </summary>
        Dictionary<string, ISqlLintRule> Rules { get; }
    }
}