using Bebbs.Monads;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using XLParser;

namespace Cells.Common.Spreadsheet.Expression
{
    public class Evaluator : Visitor<object>
    {
        public static readonly string InvalidReference = "#ref";
        public static readonly string InvalidValue = "#value";

        public enum Operation
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        private static readonly IEnumerable<(int, Type)> TypePrecidence = new (int, Type)[]
        {
            (0, typeof(string)),
            (1, typeof(double)),
            (2, typeof(float)),
            (3, typeof(decimal)),
            (4, typeof(int))
        };

        private static readonly IEnumerable<(Type, Operation, Func<object, object, object>)> Operations = new (Type, Operation, Func<object, object, object>)[]
        {
            (typeof(Int32), Operation.Add, (x, y) => Convert.ToInt32(x ?? default) + Convert.ToInt32(y ?? default)),
            (typeof(Decimal), Operation.Add, (x, y) => Convert.ToDecimal(x ?? default) + Convert.ToDecimal(y ?? default)),
            (typeof(Single), Operation.Add, (x, y) => Convert.ToSingle(x ?? default) + Convert.ToSingle(y ?? default)),
            (typeof(Double), Operation.Add, (x, y) => Convert.ToDouble(x ?? default) + Convert.ToDouble(y ?? default)),

            (typeof(Int32), Operation.Subtract, (x, y) => Convert.ToInt32(x ?? default) - Convert.ToInt32(y ?? default)),
            (typeof(Decimal), Operation.Subtract, (x, y) => Convert.ToDecimal(x ?? default) - Convert.ToDecimal(y ?? default)),
            (typeof(Single), Operation.Subtract, (x, y) => Convert.ToSingle(x ?? default) - Convert.ToSingle(y ?? default)),
            (typeof(Double), Operation.Subtract, (x, y) => Convert.ToDouble(x ?? default) - Convert.ToDouble(y ?? default)),

            (typeof(Int32), Operation.Multiply, (x, y) => Convert.ToInt32(x ?? default) * Convert.ToInt32(y ?? default)),
            (typeof(Decimal), Operation.Multiply, (x, y) => Convert.ToDecimal(x ?? default) * Convert.ToDecimal(y ?? default)),
            (typeof(Single), Operation.Multiply, (x, y) => Convert.ToSingle(x ?? default) * Convert.ToSingle(y ?? default)),
            (typeof(Double), Operation.Multiply, (x, y) => Convert.ToDouble(x ?? default) * Convert.ToDouble(y ?? default)),

            (typeof(Int32), Operation.Divide, (x, y) => Convert.ToInt32(x ?? default) / Convert.ToInt32(y ?? default)),
            (typeof(Decimal), Operation.Divide, (x, y) => Convert.ToDecimal(x ?? default) / Convert.ToDecimal(y ?? default)),
            (typeof(Single), Operation.Divide, (x, y) => Convert.ToSingle(x ?? default) / Convert.ToSingle(y ?? default)),
            (typeof(Double), Operation.Divide, (x, y) => Convert.ToDouble(x ?? default) / Convert.ToDouble(y ?? default)), // TODO: Address potential divide by zero
        };

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

        private Type GetMostAppropriateType(params object[] values)
        {
            return values
                .Where(value => value != null)
                .Select(value => value.GetType())
                .Distinct()
                .Join(TypePrecidence, type => type, tuple => tuple.Item2, (type, tuple) => tuple)
                .OrderBy(tuple => tuple.Item1)
                .Select(tuple => tuple.Item2)
                .FirstOrDefault();
        }

        private object VisitOperation(ParseTreeNode leftNode, ParseTreeNode rightNode, Operation operation)
        {
            var leftValue = Evaluate(Visit(leftNode));
            var rightValue = Evaluate(Visit(rightNode));

            var type = GetMostAppropriateType(leftValue, rightValue);

            var op = (type == null)
                ? (x, y) => InvalidValue
                : Operations
                    .Where(o => o.Item1.Equals(type) && o.Item2.Equals(operation))
                    .Select(o => o.Item3)
                    .FirstOption()
                    .Coalesce(() => (x, y) => null);

            return op(leftValue, rightValue);
        }

        private object VisitSum(ParseTreeNode argumentsNode)
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
                    .FirstOrDefault() ?? new Func<IEnumerable<object>, object>(args => InvalidReference);

                return sum(values);
            }
            else
            {
                return InvalidReference;
            }
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

        protected override object VisitFormula(ParseTreeNode node)
        {
            return Evaluate(base.VisitFormula(node));
        }

        protected override object VisitFunctionCall(ParseTreeNode node)
        {
            var function = node.GetFunction().ToUpper();

            switch (function)
            {
                case "+": return VisitOperation(node.ChildNodes[0], node.ChildNodes[2], Operation.Add);
                case "-": return VisitOperation(node.ChildNodes[0], node.ChildNodes[2], Operation.Subtract);
                case "*": return VisitOperation(node.ChildNodes[0], node.ChildNodes[2], Operation.Multiply);
                case "/": return VisitOperation(node.ChildNodes[0], node.ChildNodes[2], Operation.Divide);
                case "SUM": return VisitSum(node.ChildNodes[1]);

                default: throw new ArgumentException("node");
            }
        }

        protected override object VisitReferenceFunctionCall(ParseTreeNode node)
        {
            var function = node.GetFunction();

            switch (function)
            {
                case ":": return ReferenceRange(node.ChildNodes[0], node.ChildNodes[2]);
                default: throw new InvalidOperationException();
            }
        }

        protected override object VisitArguments(ParseTreeNode node)
        {
            return node.ChildNodes.Select(Visit).ToArray();
        }

        protected override object VisitCellToken(ParseTreeNode node)
        {
            return Index.Parse((string)node.Token.Value);
        }

        protected override object VisitNumberToken(ParseTreeNode node)
        {
            return node.Token.Value;
        }

        protected override object VisitNameToken(ParseTreeNode node)
        {
            return Index.Parse((string)node.Token.Value);
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
