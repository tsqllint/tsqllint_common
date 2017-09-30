namespace TSQLLint.Common
{
    public interface IRuleViolation
    {
        int Column { get; }
        string FileName { get; }
        int Line { get; }
        string RuleName { get; }
        RuleViolationSeverity Severity { get; }
        string Text { get; }
    }
}
