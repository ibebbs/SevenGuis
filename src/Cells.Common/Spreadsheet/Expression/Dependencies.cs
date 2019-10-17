using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using XLParser;

namespace Cells.Common.Spreadsheet.Expression
{
    public static class Dependencies
    {
        private static IEnumerable<Index> VisitFormula(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private static IEnumerable<Index> VisitConstant(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private static IEnumerable<Index> VisitNumber(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private static IEnumerable<Index> VisitReference(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private static IEnumerable<Index> ReferenceRange(ParseTreeNode fromNode, ParseTreeNode toNode)
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

        private static IEnumerable<Index> VisitReferenceFunctionCall(ParseTreeNode node)
        {
            var function = node.GetFunction();

            switch (function)
            {
                case ":": return ReferenceRange(node.ChildNodes[0], node.ChildNodes[2]);
                default: throw new InvalidOperationException();
            }
        }

        private static IEnumerable<Index> VisitNamedRange(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private static IEnumerable<Index> VisitNumberToken(ParseTreeNode node)
        {
            return Enumerable.Empty<Index>();
        }

        private static IEnumerable<Index> VisitNameToken(ParseTreeNode node)
        {
            return new[] { Index.Parse((string)node.Token.Value) };
        }

        private static IEnumerable<Index> VisitArguments(ParseTreeNode node)
        {
            return node.ChildNodes.SelectMany(child => Visit(child)).ToArray();
        }

        private static IEnumerable<Index> VisitArgument(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private static IEnumerable<Index> VisitCell(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private static IEnumerable<Index> VisitCellToken(ParseTreeNode node)
        {
            return new[] { Index.Parse((string)node.Token.Value) };
        }

        private static IEnumerable<Index> Add(ParseTreeNode leftNode, ParseTreeNode rightNode)
        {
            var leftIndexes = Visit(leftNode);
            var rightIndexes = Visit(rightNode);

            return leftIndexes.Concat(rightIndexes).ToArray();
        }

        private static IEnumerable<Index> Subtract(ParseTreeNode leftNode, ParseTreeNode rightNode)
        {
            var leftIndexes = Visit(leftNode);
            var rightIndexes = Visit(rightNode);

            return leftIndexes.Concat(rightIndexes).ToArray();
        }

        private static IEnumerable<Index> Multiply(ParseTreeNode leftNode, ParseTreeNode rightNode)
        {
            var leftIndexes = Visit(leftNode);
            var rightIndexes = Visit(rightNode);

            return leftIndexes.Concat(rightIndexes).ToArray();
        }

        private static IEnumerable<Index> Divide(ParseTreeNode leftNode, ParseTreeNode rightNode)
        {
            var leftIndexes = Visit(leftNode);
            var rightIndexes = Visit(rightNode);

            return leftIndexes.Concat(rightIndexes).ToArray();
        }

        private static IEnumerable<Index> Sum(ParseTreeNode argumentsNode)
        {
            return Visit(argumentsNode);
        }

        private static IEnumerable<Index> VisitFunctionCall(ParseTreeNode node)
        {
            var function = node.GetFunction().ToUpper();

            switch (function)
            {
                case "+": return Add(node.ChildNodes[0], node.ChildNodes[2]);
                case "-": return Subtract(node.ChildNodes[0], node.ChildNodes[2]);
                case "*": return Multiply(node.ChildNodes[0], node.ChildNodes[2]);
                case "/": return Divide(node.ChildNodes[0], node.ChildNodes[2]);
                case "SUM": return Sum(node.ChildNodes[1]);
                default: throw new ArgumentException("node");
            }
        }

        private static IEnumerable<Index> Visit(ParseTreeNode node)
        {
            switch (node.Term)
            {
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Formula): return VisitFormula(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.FunctionCall): return VisitFunctionCall(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Constant): return VisitConstant(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Number): return VisitNumber(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Reference): return VisitReference(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.ReferenceFunctionCall): return VisitReferenceFunctionCall(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.NamedRange): return VisitNamedRange(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Arguments): return VisitArguments(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Argument): return VisitArgument(node);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Cell): return VisitCell(node);
                case Terminal terminal when terminal.Name == nameof(ExcelFormulaGrammar.NumberToken): return VisitNumberToken(node);
                case Terminal terminal when terminal.Name == nameof(ExcelFormulaGrammar.NameToken): return VisitNameToken(node);
                case Terminal terminal when terminal.Name == nameof(ExcelFormulaGrammar.CellToken): return VisitCellToken(node);
                default: throw new ArgumentException("node");
            }
        }

        private static IEnumerable<Index> ExpressionReferences(string text)
        {
            if (text.StartsWith("="))
            {
                text = text.Substring(1).Trim();
            }

            var indexes = Visit(new FormulaAnalyzer(text).Root);

            return indexes;
        }

        public static IEnumerable<Index> For(string text)
        {
            return text.IsExpression()
                ? ExpressionReferences(text)
                : Enumerable.Empty<Index>();
        }
    }
}
