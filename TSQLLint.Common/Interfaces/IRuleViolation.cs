namespace TSQLLint.Common
{
    public interface IRuleViolation
    {
        int Column { get; set; }
        string FileName { get; }
        int Line { get; set;  }
        string RuleName { get; }
        RuleViolationSeverity Severity { get; }
        string Text { get; }
    }
}
