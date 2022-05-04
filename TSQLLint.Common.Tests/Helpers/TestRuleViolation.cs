namespace TSQLLint.Common.Tests.Helpers
{
    internal class TestRuleViolation : IRuleViolation
    {
        public TestRuleViolation(int line, int column = 1)
        {
            Line = line;
            Column = column;
        }

        public int Column { get; set; } = 1;

        public string FileName { get; }

        public int Line { get; set; } = 1;

        public string RuleName { get; }

        public RuleViolationSeverity Severity { get; }

        public string Text { get; }
    }
}