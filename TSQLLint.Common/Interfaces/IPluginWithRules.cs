using System;
using System.Collections.Generic;

namespace TSQLLint.Common
{
    [Obsolete("Use IPlugin and implement GetRules method instead of using the Rules property.")]
    public interface IPluginWithRules : IPlugin
    {
        /// <summary>
        /// Required if you
        /// </summary>
        Dictionary<string, ISqlLintRule> Rules { get; }

        IDictionary<string, ISqlLintRule> IPlugin.GetRules() => Rules;
    }
}