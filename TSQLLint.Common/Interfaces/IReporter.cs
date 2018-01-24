using System;

namespace TSQLLint.Common
{
    public interface IReporter : IBaseReporter
    {
        void ReportResults(TimeSpan timespan, int fileCount);
        void ReportFileResults();
        void ReportViolation(IRuleViolation violation);
        void ReportViolation(string fileName, string line, string column, string severity, string ruleName, string violationText);
    }
}
