using NUnit.Framework;
using System.Collections.Generic;

namespace TSQLLint.Common.Tests.Helpers
{
    [TestFixture]
    public partial class FixLineActionsTests
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

            // Violation on the edited line, after the removal point: its column must shift left.
            var onEditedLine = new TestRuleViolation(1, 9);
            // Control on another line whose column equals the edited line number (1):
            // it must NOT be touched by an edit to line 1.
            var onOtherLine = new TestRuleViolation(3, 1);
            Violations.Clear();
            Violations.Add(onEditedLine);
            Violations.Add(onOtherLine);

            Subject.RemoveInLine(0, 0, 4);

            Assert.AreEqual(expected, Lines[0]);
            Assert.That(onEditedLine.Column, Is.EqualTo(5));
            Assert.That(onOtherLine.Column, Is.EqualTo(1));
        }

        [Test]
        public void RemoveInLine_ViolationInsideRemovedSpan_ClampsToStart()
        {
            // Remove "This " (chars 0..4) from line 1. A violation at Column 3 ('i')
            // points inside the removed span, so its character is gone; it must clamp
            // to the start of the removal (Column 1), not go to a negative column.
            var insideSpan = new TestRuleViolation(1, 3);
            Violations.Clear();
            Violations.Add(insideSpan);

            Subject.RemoveInLine(0, 0, 5);

            // Old code did 3 - 5 = -2; clamp keeps it at charIndex + 1 = 1.
            Assert.That(insideSpan.Column, Is.EqualTo(1));
        }

        [Test]
        public void RemoveInLine_RemovalMidLine_ClampsAndShiftsAroundSpan()
        {
            // Line 1 = "Hi. This is line 1". Remove 5 chars starting at index 4 ("This ").
            // Violation at Column 6 is inside the span -> clamp to charIndex + 1 = 5.
            // Violation at Column 12 is after the span -> shift left by 5 to Column 7.
            var insideSpan = new TestRuleViolation(1, 6);
            var afterSpan = new TestRuleViolation(1, 12);
            Violations.Clear();
            Violations.Add(insideSpan);
            Violations.Add(afterSpan);

            Subject.RemoveInLine(0, 4, 5);

            Assert.That(insideSpan.Column, Is.EqualTo(5));
            Assert.That(afterSpan.Column, Is.EqualTo(7));
        }

        [Test]
        public void ReplaceAt()
        {
            var column = Lines[2].IndexOf("line") + 1;
            Violations.Add(new TestRuleViolation(3, column));

            Subject.ReplaceInlineAt(2, 0, "THIS");

            Assert.AreEqual(1, Violations[0].Column);
            Assert.AreEqual(column, Violations[1].Column);
        }

        [Test]
        public void ReplaceAtLonger()
        {
            var column = Lines[2].IndexOf("line") + 1;
            Violations.Add(new TestRuleViolation(3, column));

            Subject.ReplaceInlineAt(2, 0, "THIS", 5);

            Assert.AreEqual(1, Violations[0].Column);
            Assert.AreEqual(column - 1, Violations[1].Column);
        }

        [Test]
        public void ReplaceAtGrowing()
        {
            // "This is line 3": violation on the 'i' of "This" (0-based index 2 => Column 3).
            Violations.Add(new TestRuleViolation(3, 3));

            // Replace the 2 chars "Th" with 6 chars: net growth of +4.
            Subject.ReplaceInlineAt(2, 0, "XXXXXX", 2);

            // The default violation at Column 1 sits inside the replaced region and is not shifted.
            Assert.That(Violations[0].Column, Is.EqualTo(1));
            // The 'i' moves from Column 3 to Column 7; the buggy threshold left it at 3.
            Assert.That(Violations[1].Column, Is.EqualTo(7));
        }

        [Test]
        public void RepaceInlineAt_Obsolete_DelegatesToReplaceInlineAt()
        {
            var column = Lines[2].IndexOf("line") + 1;
            Violations.Add(new TestRuleViolation(3, column));

            // The misspelled method is obsolete but must still behave identically.
#pragma warning disable CS0618 // Type or member is obsolete
            Subject.RepaceInlineAt(2, 0, "THIS");
#pragma warning restore CS0618

            Assert.That(Lines[2], Is.EqualTo("THIS is line 3"));
            Assert.That(Violations[0].Column, Is.EqualTo(1));
            Assert.That(Violations[1].Column, Is.EqualTo(column));
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

        [Test]
        public void UpdateLine()
        {
            var content = "Replacement content";

            Subject.UpdateLine(2, content);

            Assert.That(Lines[2], Is.EqualTo(content));
            // UpdateLine replaces the whole line and does not touch violation positions.
            Assert.That(Violations[0].Line, Is.EqualTo(3));
            Assert.That(Violations[0].Column, Is.EqualTo(1));
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
                new TestRuleViolation(3)
            };
        }
    }
}