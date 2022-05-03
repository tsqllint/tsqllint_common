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
        public void Insert()
        {
            var line = Violations[0].Line + 1;
            var content = "This is line 0";

            Subject.Insert(0, content);

            Assert.AreEqual(content, Lines[0]);
            Assert.AreEqual(line, Violations[0].Line);
        }

        [Test]
        public void InsertInLine()
        {
            var oldColumn = Violations[0].Column;
            var line = Lines[2];
            var content = "Prefix this";

            Subject.InsertInLine(2, 0, content);

            Assert.AreEqual(content + line, Lines[2]);
            Assert.AreEqual(content.Length + oldColumn, Violations[0].Column);
        }

        [Test]
        public void InsertRange()
        {
            Subject.InsertRange(2, new[] { "This is line 2.1", "This is line 2.2" });

            Assert.AreEqual(5, Violations[0].Line);
        }

        [Test]
        public void InsertRangeAfter()
        {
            Subject.InsertRange(3, new[] { "This is line 2.1", "This is line 2.2" });

            Assert.AreEqual(3, Violations[0].Line);
        }

        [Test]
        public void RemoveAll()
        {
            Subject.RemoveAll(x => x.Contains("This is line"));

            Assert.AreEqual(1, Lines.Count);
        }

        [Test]
        public void RemoveAt()
        {
            var expectedLineCount = Lines.Count - 1;
            var errorLine = Violations[0].Line;

            Subject.RemoveAt(0);

            Assert.AreEqual(expectedLineCount, Lines.Count);
            Assert.AreEqual(errorLine - 1, Violations[0].Line);
        }

        [Test]
        public void RemoveInLine()
        {
            var expected = Lines[0].Substring(4);

            Subject.RemoveInLine(0, 0, 4);

            Assert.AreEqual(expected, Lines[0]);
        }

        [Test]
        public void ReplaceAt()
        {
            var column = Lines[2].IndexOf("line") + 1;
            Violations.Add(new RuleViolation(3, column));

            Subject.RepaceInlineAt(2, 0, "THIS");

            Assert.AreEqual(1, Violations[0].Column);
            Assert.AreEqual(column, Violations[1].Column);
        }

        [Test]
        public void ReplaceAtLonger()
        {
            var column = Lines[2].IndexOf("line") + 1;
            Violations.Add(new RuleViolation(3, column));

            Subject.RepaceInlineAt(2, 0, "THIS", 5);

            Assert.AreEqual(1, Violations[0].Column);
            Assert.AreEqual(column - 1, Violations[1].Column);
        }

        [Test]
        public void RemoveRange()
        {
            var count = Lines.Count;
            var errorLine = Violations[0].Line;
            var remove = 2;

            Subject.RemoveRange(2, remove);

            Assert.AreEqual(count - remove, Lines.Count);
            Assert.AreEqual(errorLine - remove, Violations[0].Line);
        }

        [Test]
        public void RemoveRangeAfter()
        {
            var count = Lines.Count;
            var remove = 2;

            Subject.RemoveRange(3, remove);

            Assert.AreEqual(count - remove, Lines.Count);
            Assert.AreEqual(3, Violations[0].Line);
        }

        [Test]
        public void RemovesLines()
        {
            Subject.RemoveRange(0, 2);

            Assert.AreEqual(1, Violations[0].Line);
        }

        private void SetupDefaultLines()
        {
            Lines = new List<string>
            {
                "Hi. This is line 1",
                "This is line 2",
                "This is line 3",
                "This is line 4",
                "This is not line 4"
            };
        }

        private void SetupDefaultViolations()
        {
            Violations = new List<IRuleViolation>
            {
                new RuleViolation(3)
            };
        }

        private class RuleViolation : IRuleViolation
        {
            public RuleViolation(int line, int column = 1)
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
}