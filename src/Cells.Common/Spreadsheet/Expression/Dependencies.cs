using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using XLParser;

namespace Cells.Common.Spreadsheet.Expression
{
    public class Dependencies : Visitor<IEnumerable<Index>>
    {
        public static readonly Dependencies Instance = new Dependencies();

        private IEnumerable<Index> ReferenceRange(ParseTreeNode fromNode, ParseTreeNode toNode)
        {
            var fromIndex = Visit(fromNode).First();
            var toIndex = Visit(toNode).First();

            return Enumerable
                .Range(fromIndex.Row, toIndex.Row - fromIndex.Row + 1)
                .SelectMany(row => Enumerable
                    .Range((int)fromIndex.Column, (int)toIndex.Column - (int)fromIndex.Column + 1)
                    .Select(column => new Index(row, (char)column)))
                .ToArray();
        }

        private IEnumerable<Index> VisitNodes(IEnumerable<ParseTreeNode> nodes)
        {
            return nodes.SelectMany(Visit).ToArray();
        }

        protected override IEnumerable<Index> VisitReferenceFunctionCall(ParseTreeNode node)
        {
            var function = node.GetFunction();

            switch (function)
            {
                case ":": return ReferenceRange(node.ChildNodes[0], node.ChildNodes[2]);
                default: throw new InvalidOperationException();
            }
        }

        protected override IEnumerable<Index> VisitNumberToken(ParseTreeNode node)
        {
            return Enumerable.Empty<Index>();
        }

        protected override IEnumerable<Index> VisitNameToken(ParseTreeNode node)
        {
            return new[] { Index.Parse((string)node.Token.Value) };
        }

        protected override IEnumerable<Index> VisitArguments(ParseTreeNode node)
        {
            return node.ChildNodes.SelectMany(child => Visit(child)).ToArray();
        }

        protected override IEnumerable<Index> VisitCellToken(ParseTreeNode node)
        {
            return new[] { Index.Parse((string)node.Token.Value) };
        }

        protected override IEnumerable<Index> VisitFunctionCall(ParseTreeNode node)
        {
            var function = node.GetFunction().ToUpper();

            switch (function)
            {
                case "+": return VisitNodes(node.GetFunctionArguments());
                case "-": return VisitNodes(node.GetFunctionArguments());
                case "*": return VisitNodes(node.GetFunctionArguments());
                case "/": return VisitNodes(node.GetFunctionArguments());
                case "SUM": return VisitNodes(node.GetFunctionArguments());
                default: throw new ArgumentException("node");
            }
        }

        private IEnumerable<Index> ExpressionReferences(string text)
        {
            if (text.StartsWith("="))
            {
                text = text.Substring(1).Trim();
            }

            var indexes = Visit(new FormulaAnalyzer(text).Root);

            return indexes;
        }

        public IEnumerable<Index> For(string text)
        {
            return text.IsExpression()
                ? ExpressionReferences(text)
                : Enumerable.Empty<Index>();
        }
    }
}
