using System.Collections.Generic;

namespace TSQLLint.Common
{
    public interface ISqlLintRule
    {
        string RULE_NAME { get; }

        RuleViolationSeverity RULE_SEVERITY { get; }

        public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions);
    }
}