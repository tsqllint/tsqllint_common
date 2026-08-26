using System;
using System.Collections.Generic;
using System.Linq;

namespace TSQLLint.Common
{
    public class FileLineActions
    {
        private readonly List<string> FileLines;

        private readonly List<IRuleViolation> RuleViolations;

        public FileLineActions(List<IRuleViolation> ruleViolations, List<string> fileLines)
        {
            RuleViolations = ruleViolations;
            FileLines = fileLines;
        }

        public void Insert(int index, string line)
        {
            InsertRange(index, new[] { line });
        }

        public void InsertInLine(int lineIndex, int charIndex, string content)
        {
            var line = FileLines[lineIndex];
            line = line.Insert(charIndex, content);
            FileLines[lineIndex] = line;

            foreach (var v in RuleViolations.Where(x => x.Line == lineIndex + 1
                && x.Column > charIndex))
            {
                v.Column += content.Length;
            }
        }

        public void InsertRange(int index, IList<string> lines)
        {
            FileLines.InsertRange(index, lines);

            foreach (var v in RuleViolations.Where(x => x.Line > index))
            {
                v.Line += lines.Count;
            }
        }

        public void RemoveAll(Func<string, bool> where)
        {
            for (var index = FileLines.Count - 1; index >= 0; index--)
            {
                if (where(FileLines[index]))
                {
                    RemoveAt(index);
                }
            }
        }

        public void RemoveAt(int index)
        {
            RemoveRange(index, 1);
        }

        public void RemoveInLine(int lineIndex, int charIndex, int length)
        {
            var line = FileLines[lineIndex];
            line = line.Remove(charIndex, length);
            FileLines[lineIndex] = line;

            foreach (var v in RuleViolations.Where(x => x.Line == lineIndex + 1
                && x.Column > charIndex))
            {
                // Violations after the removed span shift left by length; violations
                // inside the span (their character was deleted) clamp to the start of
                // the removal so the column can never become zero or negative.
                v.Column = Math.Max(charIndex + 1, v.Column - length);
            }
        }

        public void RemoveRange(int index, int count)
        {
            FileLines.RemoveRange(index, count);

            foreach (var v in RuleViolations.Where(x => x.Line > index))
            {
                v.Line -= count;
            }
        }

        [Obsolete("Use ReplaceInlineAt instead. This method is misspelled and will be removed in a future release.")]
        public void RepaceInlineAt(int lineIndex, int charIndex, string content, int? replaceLength = null)
        {
            ReplaceInlineAt(lineIndex, charIndex, content, replaceLength);
        }

        public void ReplaceInlineAt(int lineIndex, int charIndex, string content, int? replaceLength = null)
        {
            var line = FileLines[lineIndex];
            line = line.Remove(charIndex, replaceLength ?? content.Length);
            line = line.Insert(charIndex, content);
            FileLines[lineIndex] = line;

            if (replaceLength.HasValue && replaceLength != content.Length)
            {
                var lengthDiff = content.Length - replaceLength;
                foreach (var v in RuleViolations.Where(x => x.Line == lineIndex + 1
                    && x.Column > charIndex + replaceLength.Value))
                {
                    v.Column += lengthDiff.Value;
                }
            }
        }

        public void UpdateLine(int lineIndex, string content)
        {
            FileLines[lineIndex] = content;
        }
    }
}