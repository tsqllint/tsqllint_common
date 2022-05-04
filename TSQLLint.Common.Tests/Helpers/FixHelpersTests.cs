using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TSQLLint.Common.Tests.Helpers
{
    [TestFixture]
    internal class FixHelpersTests
    {
        [Test]
        [TestCase("    4 spaces", "    ")]
        [TestCase("\t\t2 tabs", "\t\t")]
        public void GetIndent_Lines(string line, string expected)
        {
            var lines = new[] { line }.ToList();
            var violation = new TestRuleViolation(1);
            var result = FixHelpers.GetIndent(lines, violation);
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase("    4 spaces", "    ")]
        [TestCase("\t\t2 tabs", "\t\t")]
        public void GetIndent_SqlStatement(string line, string expected)
        {
            var tsqlStatement = new IfStatement()
            {
                FirstTokenIndex = 0,
                ScriptTokenStream = new List<TSqlParserToken>()
                {
                    // only line matters
                    new TSqlParserToken(TSqlTokenType.If, 0, "doesn't matter", line: 1, 1)
                }
            };

            var lines = new[] { line }.ToList();
            var result = FixHelpers.GetIndent(lines, tsqlStatement);
            Assert.AreEqual(expected, result);
        }
    }
}