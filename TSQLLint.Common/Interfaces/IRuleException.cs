namespace TSQLLint.Common
{
    public interface IRuleException
    {
        int StartLine { get; }
        int EndLine { get; }
        string RuleName { get; }
    }
}