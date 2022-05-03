using NUnit.Framework;
using System.Collections.Generic;

namespace TSQLLint.Common.Tests.Helpers
{
    [TestFixture]
    public class FixLineActionsTests
    {
        private List<string> Lines;
        private FileLineActions Subject;
        private List<IRuleViolation> Violations;

        [SetUp]
        public void Init()
        {
            SetupDefaultLines();
            SetupDefaultViolations();
            Subject = new FileLineActions(Violations, Lines);
        }

        [Test]
        [TestOf(nameof(FileLineActions.InsertRange))]
        public void InsertRange()
        {
            Subject.InsertRange(2, new[] { "This is line 2.1", "This is line 2.2" });

            Assert.AreEqual(5, Violations[0].Line);
        }

        [Test]
        [TestOf(nameof(FileLineActions.RemoveRange))]
        public void RemovesLines()
        {
            Subject.RemoveRange(0, 2);

            Assert.AreEqual(1, Violations[0].Line);
        }

        private void SetupDefaultLines()
        {
            Lines = new List<string>
            {
                "This is line 1",
                "This is line 2",
                "This is line 3",
                "This is line 4",
                "This is line 5"
            };
        }

        private void SetupDefaultViolations()
        {
            Violations = new List<IRuleViolation>
            {
                new RuleViolation(3, "Error")
            };
        }

        private class RuleViolation : IRuleViolation
        {
            public RuleViolation(int line, string text)
            {
                Line = line;
                Text = text;
            }

            public int Column { get; }

            public string FileName { get; }

            public int Line { get; set; }

            public string RuleName { get; }

            public RuleViolationSeverity Severity { get; }

            public string Text { get; }
        }
    }
}