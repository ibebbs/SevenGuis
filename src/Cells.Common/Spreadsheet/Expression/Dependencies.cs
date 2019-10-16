using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using XLParser;

namespace Cells.Common.Spreadsheet.Expression
{
    public static class Dependencies
    {
        private static IEnumerable<Index> VisitFormula(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            return Visit(node.ChildNodes[0], indexes);
        }

        private static IEnumerable<Index> VisitConstant(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            return Visit(node.ChildNodes[0], indexes);
        }

        private static IEnumerable<Index> VisitNumber(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            return Visit(node.ChildNodes[0], indexes);
        }

        private static IEnumerable<Index> VisitReference(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            return Visit(node.ChildNodes[0], indexes);
        }

        private static IEnumerable<Index> ReferenceRange(ParseTreeNode fromNode, ParseTreeNode toNode, IEnumerable<Index> indexes)
        {
            var fromIndex = Visit(fromNode, Enumerable.Empty<Index>()).First();
            var toIndex = Visit(toNode, Enumerable.Empty<Index>()).First();

            return Enumerable
                .Range(fromIndex.Row, toIndex.Row - fromIndex.Row + 1)
                .SelectMany(row => Enumerable
                    .Range((int)fromIndex.Column, (int)toIndex.Column - (int)fromIndex.Column + 1)
                    .Select(column => new Index(row, (char)column)))
                .ToArray();
        }

        private static IEnumerable<Index> VisitReferenceFunctionCall(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            var function = node.GetFunction();

            switch (function)
            {
                case ":": return ReferenceRange(node.ChildNodes[0], node.ChildNodes[2], indexes);
                default: throw new InvalidOperationException();
            }
        }

        private static IEnumerable<Index> VisitNamedRange(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            return Visit(node.ChildNodes[0], indexes);
        }

        private static IEnumerable<Index> VisitNumberToken(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            return Enumerable.Empty<Index>();
        }

        private static IEnumerable<Index> VisitNameToken(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            return indexes.Concat(new[] { Index.Parse((string)node.Token.Value) });
        }

        private static IEnumerable<Index> VisitArguments(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            return node.ChildNodes.SelectMany(child => Visit(child, indexes)).ToArray();
        }

        private static IEnumerable<Index> VisitArgument(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            return Visit(node.ChildNodes[0], indexes);
        }

        private static IEnumerable<Index> VisitCell(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            return Visit(node.ChildNodes[0], indexes);
        }

        private static IEnumerable<Index> VisitCellToken(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            return indexes.Concat(new[] { Index.Parse((string)node.Token.Value) });
        }

        private static IEnumerable<Index> Add(ParseTreeNode leftNode, ParseTreeNode rightNode, IEnumerable<Index> indexes)
        {
            var leftIndexes = Visit(leftNode, Enumerable.Empty<Index>());
            var rightIndexes = Visit(rightNode, Enumerable.Empty<Index>());

            return leftIndexes.Concat(rightIndexes).ToArray();
        }

        private static IEnumerable<Index> Subtract(ParseTreeNode leftNode, ParseTreeNode rightNode, IEnumerable<Index> indexes)
        {
            var leftIndexes = Visit(leftNode, Enumerable.Empty<Index>());
            var rightIndexes = Visit(rightNode, Enumerable.Empty<Index>());

            return leftIndexes.Concat(rightIndexes).ToArray();
        }

        private static IEnumerable<Index> Multiply(ParseTreeNode leftNode, ParseTreeNode rightNode, IEnumerable<Index> indexes)
        {
            var leftIndexes = Visit(leftNode, Enumerable.Empty<Index>());
            var rightIndexes = Visit(rightNode, Enumerable.Empty<Index>());

            return leftIndexes.Concat(rightIndexes).ToArray();
        }

        private static IEnumerable<Index> Divide(ParseTreeNode leftNode, ParseTreeNode rightNode, IEnumerable<Index> indexes)
        {
            var leftIndexes = Visit(leftNode, Enumerable.Empty<Index>());
            var rightIndexes = Visit(rightNode, Enumerable.Empty<Index>());

            return leftIndexes.Concat(rightIndexes).ToArray();
        }

        private static IEnumerable<Index> Sum(ParseTreeNode argumentsNode, IEnumerable<Index> indexes)
        {
            return Visit(argumentsNode, indexes);
        }

        private static IEnumerable<Index> VisitFunctionCall(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            var function = node.GetFunction().ToUpper();

            switch (function)
            {
                case "+": return Add(node.ChildNodes[0], node.ChildNodes[2], indexes);
                case "-": return Subtract(node.ChildNodes[0], node.ChildNodes[2], indexes);
                case "*": return Multiply(node.ChildNodes[0], node.ChildNodes[2], indexes);
                case "/": return Divide(node.ChildNodes[0], node.ChildNodes[2], indexes);
                case "SUM": return Sum(node.ChildNodes[1], indexes);
                default: throw new ArgumentException("node");
            }
        }

        private static IEnumerable<Index> Visit(ParseTreeNode node, IEnumerable<Index> indexes)
        {
            switch (node.Term)
            {
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Formula): return VisitFormula(node, indexes);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.FunctionCall): return VisitFunctionCall(node, indexes);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Constant): return VisitConstant(node, indexes);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Number): return VisitNumber(node, indexes);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Reference): return VisitReference(node, indexes);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.ReferenceFunctionCall): return VisitReferenceFunctionCall(node, indexes);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.NamedRange): return VisitNamedRange(node, indexes);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Arguments): return VisitArguments(node, indexes);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Argument): return VisitArgument(node, indexes);
                case NonTerminal nonTerminal when nonTerminal.Name == nameof(ExcelFormulaGrammar.Cell): return VisitCell(node, indexes);
                case Terminal terminal when terminal.Name == nameof(ExcelFormulaGrammar.NumberToken): return VisitNumberToken(node, indexes);
                case Terminal terminal when terminal.Name == nameof(ExcelFormulaGrammar.NameToken): return VisitNameToken(node, indexes);
                case Terminal terminal when terminal.Name == nameof(ExcelFormulaGrammar.CellToken): return VisitCellToken(node, indexes);
                default: throw new ArgumentException("node");
            }
        }

        private static IEnumerable<Index> ExpressionReferences(string text)
        {
            if (text.StartsWith("="))
            {
                text = text.Substring(1).Trim();
            }

            var indexes = Visit(new FormulaAnalyzer(text).Root, Enumerable.Empty<Index>());

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
