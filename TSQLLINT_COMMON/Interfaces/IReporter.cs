using System;

namespace TSQLLINT_COMMON
{
    public interface IReporter : IBaseReporter
    {
        void ReportResults(TimeSpan timespan, int fileCount);
        void ReportViolation(IRuleViolation violation);
        void ReportViolation(string fileName, string line, string column, string severity, string ruleName, string violationText);
    }
}
