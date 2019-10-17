using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cells.Common.Tests.Spreadsheet.Expression
{
    [TestFixture]
    public class EvaluatorShould
    {
        private static IEnumerable<TestCaseData> EvaluationTestCases
        {
            get
            {
                yield return new TestCaseData("= 314", Enumerable.Empty<(Common.Spreadsheet.Index, object)>())
                    .SetName("Evaluate constant")
                    .Returns(314);
                yield return new TestCaseData("= A0", new[] { (new Common.Spreadsheet.Index(0, 'A'), (object)3.14M) })
                    .SetName("Evaluate cell reference with value")
                    .Returns(3.14M);
                yield return new TestCaseData("= A0", Enumerable.Empty<(Common.Spreadsheet.Index, object)>())
                    .SetName("Evaluate cell reference without value")
                    .Returns(null);
                yield return new TestCaseData("= A0 + 1", new[] { (new Common.Spreadsheet.Index(0, 'A'), (object)3.14M) })
                    .SetName("Evaluate cell reference with value added to constant")
                    .Returns(4.14M);
                yield return new TestCaseData("= A0 + 1", Enumerable.Empty<(Common.Spreadsheet.Index, object)>())
                    .SetName("Evaluate cell reference without value added to constant")
                    .Returns(1);
                yield return new TestCaseData("= A0 + B0", new[] { (new Common.Spreadsheet.Index(0, 'A'), (object)3.14M), (new Common.Spreadsheet.Index(0, 'B'), (object)1.59M) })
                    .SetName("Evaluate cell reference with value added to cell reference with value")
                    .Returns(4.73M);
                yield return new TestCaseData("= A0 + B0", new[] { (new Common.Spreadsheet.Index(0, 'A'), (object)3.14M) })
                    .SetName("Evaluate cell reference with value added to cell reference without value")
                    .Returns(3.14M);
                yield return new TestCaseData("= A0 + B0", new[] { (new Common.Spreadsheet.Index(0, 'B'), (object)1.59M) })
                    .SetName("Evaluate cell reference without value added to cell reference with value")
                    .Returns(1.59M);
                yield return new TestCaseData("= A0 + B0", new[] { (new Common.Spreadsheet.Index(0, 'A'), (object)3.14M), (new Common.Spreadsheet.Index(0, 'B'), (object)2) })
                    .SetName("Evaluate cell reference with value added to cell reference with different value type")
                    .Returns(5.14M);
                yield return new TestCaseData("= SUM(A0:B0)", new[] { (new Common.Spreadsheet.Index(0, 'A'), (object)3.14M), (new Common.Spreadsheet.Index(0, 'B'), (object)1.59M) })
                    .SetName("Evaluate sum of cell references when cells have values")
                    .Returns(4.73M);
                yield return new TestCaseData("= SUM(A0:B0)", new[] { (new Common.Spreadsheet.Index(0, 'B'), (object)1.59M) })
                    .SetName("Evaluate sum of cell references when a cell doesn't have a value")
                    .Returns(1.59M);
            }
        }

        [TestCaseSource(nameof(EvaluationTestCases))]
        public object ReturnTheExpectedValue(string expression, IEnumerable<(Common.Spreadsheet.Index, object)> values)
        {
            var valueDictionary = values.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);

            var subject = new Common.Spreadsheet.Expression.Evaluator(index => valueDictionary.TryGetValue(index, out object value) ? value : null);

            var actual = subject.Evaluate(expression);

            return actual;
        }
    }
}
