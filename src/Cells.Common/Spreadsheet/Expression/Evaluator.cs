using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLParser;

namespace Cells.Common.Spreadsheet.Expression
{
    public class Evaluator
    {
        private static readonly IEnumerable<(Type, int, Func<IEnumerable<object>, object>)> SumTypes = new[]
        {
            (typeof(string), 0, new Func<IEnumerable<object>, object>(args => "#ref")),
            (typeof(int), 1, new Func<IEnumerable<object>, object>(args => args.OfType<int>().Sum())),
            (typeof(decimal), 2, new Func<IEnumerable<object>, object>(args => args.OfType<decimal>().Sum())),
            (typeof(float), 3, new Func<IEnumerable<object>, object>(args => args.OfType<float>().Sum())),
            (typeof(double), 4, new Func<IEnumerable<object>, object>(args => args.OfType<double>().Sum()))
        };

        private readonly Func<Index, object> _lookup;

        public Evaluator(Func<Index, object> lookup)
        {
            _lookup = lookup;
        }

        private object VisitFormula(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private object VisitConstant(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private object VisitNumber(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private object VisitReference(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private object ReferenceRange(ParseTreeNode fromNode, ParseTreeNode toNode)
        {
            var fromIndex = (Index)Visit(fromNode);
            var toIndex = (Index)Visit(toNode);

            return Enumerable
                .Range(fromIndex.Row, toIndex.Row - fromIndex.Row + 1)
                .SelectMany(row => Enumerable
                    .Range((int)fromIndex.Column, (int)toIndex.Column - (int)fromIndex.Column + 1)
                    .Select(column => new Index(row, (char)column)))
                .Select(index => Evaluate(index))
                .ToArray();
        }

        private object VisitReferenceFunctionCall(ParseTreeNode node)
        {
            var function = node.GetFunction();

            switch (function)
            {
                case ":": return ReferenceRange(node.ChildNodes[0], node.ChildNodes[2]);
                default: throw new InvalidOperationException();
            }
        }

        private object VisitNamedRange(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private object VisitNumberToken(ParseTreeNode node)
        {
            return node.Token.Value;
        }

        private object VisitNameToken(ParseTreeNode node)
        {
            return Index.Parse((string)node.Token.Value);
        }

        private object VisitArguments(ParseTreeNode node)
        {
            return node.ChildNodes.Select(Visit).ToArray();
        }

        private object VisitArgument(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private object VisitCell(ParseTreeNode node)
        {
            return Visit(node.ChildNodes[0]);
        }

        private object VisitCellToken(ParseTreeNode node)
        {
            return Index.Parse((string)node.Token.Value);
        }

        private object Add(ParseTreeNode leftNode, ParseTreeNode rightNode)
        {
            var leftValue = Evaluate(Visit(leftNode));
            var rightValue = Evaluate(Visit(rightNode));

            switch (leftValue)
            {
                case Int32 left when rightValue is Int32: return left + Convert.ToInt32(rightValue);
                case decimal left: return left + Convert.ToDecimal(rightValue);
                case float left: return left + Convert.ToSingle(rightValue);
                case double left: return left + Convert.ToDouble(rightValue);
                default: throw new InvalidOperationException();
            }
        }

        private object Subtract(ParseTreeNode leftNode, ParseTreeNode rightNode)
        {
            var leftValue = Evaluate(Visit(leftNode));
            var rightValue = Evaluate(Visit(rightNode));

            switch (leftValue)
            {
                case Int32 left when rightValue is Int32: return left - Convert.ToInt32(rightValue);
                case decimal left: return left - Convert.ToDecimal(rightValue);
                case float left: return left - Convert.ToSingle(rightValue);
                case double left: return left - Convert.ToDouble(rightValue);
                default: throw new InvalidOperationException();
            }
        }

        private object Multiply(ParseTreeNode leftNode, ParseTreeNode rightNode)
        {
            var leftValue = Evaluate(Visit(leftNode));
            var rightValue = Evaluate(Visit(rightNode));

            switch (leftValue)
            {
                case Int32 left when rightValue is Int32: return left * Convert.ToInt32(rightValue);
                case decimal left: return left * Convert.ToDecimal(rightValue);
                case float left: return left * Convert.ToSingle(rightValue);
                case double left: return left * Convert.ToDouble(rightValue);
                default: throw new InvalidOperationException();
            }
        }

        private object Divide(ParseTreeNode leftNode, ParseTreeNode rightNode)
        {
            var leftValue = Evaluate((Index)Visit(leftNode));
            var rightValue = Evaluate((Index)Visit(rightNode));

            switch (leftValue)
            {
                case Int32 left when rightValue is Int32: return left / Convert.ToInt32(rightValue);
                case decimal left: return left / Convert.ToDecimal(rightValue);
                case float left: return left / Convert.ToSingle(rightValue);
                case double left: return left / Convert.ToDouble(rightValue);
                default: throw new InvalidOperationException();
            }
        }

        private object Sum(ParseTreeNode argumentsNode)
        {
            var arguments = Visit(argumentsNode) as object[];
            var values = arguments[0] as IEnumerable<object>;

            if (values.Any())
            {
                var sum = values
                    .Where(value => value != null)
                    .Select(value => value.GetType())
                    .Distinct()
                    .Join(SumTypes, type => type, tuple => tuple.Item1, (type, tuple) => tuple)
                    .OrderBy(tuple => tuple.Item2)
                    .Select(tuple => tuple.Item3)
                    .FirstOrDefault() ?? new Func<IEnumerable<object>, object>(args => "#ref");

                return sum(values);
            }
            else
            {
                return "#ref";
            }
        }

        private object VisitFunctionCall(ParseTreeNode node)
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


        private object Visit(ParseTreeNode node)
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

        private object Evaluate(object source)
        {
            switch (source)
            {
                case Index index: return _lookup.Invoke(index);
                default: return source;
            }
        }

        public object Evaluate(string text)
        {
            if (text.StartsWith("="))
            {
                text = text.Substring(1).Trim();
            }

            var fa = new FormulaAnalyzer(text);

            return Visit(fa.Root);
        }
    }
}
