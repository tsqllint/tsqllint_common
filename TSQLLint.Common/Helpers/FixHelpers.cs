using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TSQLLint.Common
{
    public static class FixHelpers
    {
        public static (TReturn, TFind) FindViolatingNode<TFind, TReturn>(
             List<string> fileLines, IRuleViolation ruleViolation, Func<TFind, TReturn> getFragment)
             where TFind : TSqlFragment
             where TReturn : TSqlFragment
        {
            var node = FindNodes<TFind>(fileLines).FirstOrDefault(x =>
            {
                var fragment = getFragment(x);
                return fragment?.StartLine == ruleViolation.Line &&
                       fragment?.StartColumn == ruleViolation.Column;
            });

            return (getFragment(node), node);
        }

        public static List<T> FindNodes<T>(List<string> fileLines, Func<T, bool> where = null)
             where T : TSqlFragment
        {
            using var rdr = new StringReader(string.Join("\n", fileLines));
            var parser = new TSql150Parser(true, SqlEngineType.All);
            var tree = parser.Parse(rdr, out var errors);

            if (errors?.Any() == true)
            {
                throw new Exception($"Parsing failed. {string.Join(". ", errors.Select(x => x.Message))}");
            }

            var checker = new FindViolatingNodeVisitor<T>(where);
            tree.Accept(checker);

            return checker.Nodes;
        }

        public static List<T> FindNodes<T>(TSqlStatement statement, Func<T, bool> where = null)
            where T : TSqlFragment
        {
            var checker = new FindViolatingNodeVisitor<T>(where);
            statement.Accept(checker);
            return checker.Nodes;
        }

        public static T FindViolatingNode<T>(List<string> fileLines, IRuleViolation ruleViolation)
            where T : TSqlFragment
        {
            return FindViolatingNode<T, T>(fileLines, ruleViolation, x => x).Item1;
        }

        public static string GetIndent(List<string> fileLines, IRuleViolation ruleViolation)
        {
            return GetIndent(fileLines[ruleViolation.Line - 1]);
        }

        public static string GetIndent(List<string> fileLines, TSqlStatement statement)
        {
            return GetIndent(fileLines[statement.StartLine - 1]);
        }

        private static string GetIndent(string ifLine)
        {
            var ifPrefix = new Regex(@"^\s+").Match(ifLine);

            var indent = string.Empty;
            if (ifPrefix.Success)
            {
                indent = ifPrefix.Value;
            }

            return indent;
        }

        public class FindViolatingNodeVisitor<T> : TSqlFragmentVisitor
            where T : TSqlFragment
        {
            private readonly Func<T, bool> Where;
            public List<T> Nodes = new List<T>();

            public FindViolatingNodeVisitor(Func<T, bool> where = null)
            {
                Where = where;
            }

            public override void Visit(TSqlFragment node)
            {
                if (node is T fragment && (Where == null || Where(fragment)))
                {
                    Nodes.Add(fragment);
                }

                base.Visit(node);
            }
        }
    }
}