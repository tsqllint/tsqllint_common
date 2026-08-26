using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using System;
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

        [Test]
        public void FindViolatingNode_NoMatch_ReturnsDefaultWithoutThrowing()
        {
            var lines = new[] { "SELECT 1" }.ToList();
            // A position that matches no fragment's start.
            var violation = new TestRuleViolation(99, 99);

            // getFragment dereferences its argument; before the fix this threw
            // NullReferenceException because FirstOrDefault returned null.
            var (fragment, node) = FixHelpers.FindViolatingNode<SelectStatement, QueryExpression>(
                lines, violation, x => x.QueryExpression);

            Assert.That(fragment, Is.Null);
            Assert.That(node, Is.Null);
        }

        [Test]
        public void FindViolatingNode_Match_ReturnsNodeAndFragment()
        {
            var lines = new[] { "SELECT 1" }.ToList();
            // The SELECT statement's query expression starts at line 1, column 1.
            var violation = new TestRuleViolation(1, 1);

            var (fragment, node) = FixHelpers.FindViolatingNode<SelectStatement, QueryExpression>(
                lines, violation, x => x.QueryExpression);

            Assert.That(node, Is.Not.Null);
            Assert.That(fragment, Is.SameAs(node.QueryExpression));
        }

        [Test]
        public void FindNodes_ParsesSqlServer2022Syntax()
        {
            // A named WINDOW clause is valid T-SQL 2022 (TSql160); TSql150Parser
            // rejects it as a syntax error, which previously aborted the fix.
            var lines = new[] { "SELECT c, SUM(c) OVER w FROM t WINDOW w AS (ORDER BY c);" }.ToList();

            var nodes = FixHelpers.FindNodes<SelectStatement>(lines);

            Assert.That(nodes.Count, Is.EqualTo(1));
        }

        [Test]
        public void FindNodes_InvalidSql_Throws()
        {
            var lines = new[] { "SELECT FROM WHERE ((" }.ToList();

            Assert.Throws<Exception>(() => FixHelpers.FindNodes<SelectStatement>(lines));
        }

        [Test]
        public void FindNodes_FromFragment_FindsDescendants()
        {
            var lines = new[] { "SELECT a, b FROM t" }.ToList();
            var statement = FixHelpers.FindNodes<SelectStatement>(lines).Single();

            // The fragment overload walks the already-parsed subtree.
            var columns = FixHelpers.FindNodes<ColumnReferenceExpression>(statement);

            Assert.That(columns.Count, Is.EqualTo(2));
        }

        [Test]
        public void FindNodes_FromFragment_HonoursWherePredicate()
        {
            var lines = new[] { "SELECT a, b FROM t" }.ToList();
            var statement = FixHelpers.FindNodes<SelectStatement>(lines).Single();

            var columns = FixHelpers.FindNodes<ColumnReferenceExpression>(
                statement,
                c => FixHelpers.GetString(c) == "b");

            Assert.That(columns.Count, Is.EqualTo(1));
        }

        [Test]
        public void FindViolatingNode_SingleType_ReturnsMatchingNode()
        {
            var lines = new[] { "SELECT 1" }.ToList();
            // The SELECT statement starts at line 1, column 1.
            var violation = new TestRuleViolation(1, 1);

            var node = FixHelpers.FindViolatingNode<SelectStatement>(lines, violation);

            Assert.That(node, Is.Not.Null);
        }

        [Test]
        public void FindViolatingNode_SingleType_NoMatch_ReturnsNull()
        {
            var lines = new[] { "SELECT 1" }.ToList();
            var violation = new TestRuleViolation(99, 99);

            var node = FixHelpers.FindViolatingNode<SelectStatement>(lines, violation);

            Assert.That(node, Is.Null);
        }

        [Test]
        public void GetString_ReconstructsFragmentText()
        {
            var lines = new[] { "SELECT a, b FROM t" }.ToList();
            var column = FixHelpers.FindNodes<ColumnReferenceExpression>(lines)
                .Single(c => c.ScriptTokenStream[c.FirstTokenIndex].Text == "b");

            var result = FixHelpers.GetString(column);

            Assert.That(result, Is.EqualTo("b"));
        }
    }
}