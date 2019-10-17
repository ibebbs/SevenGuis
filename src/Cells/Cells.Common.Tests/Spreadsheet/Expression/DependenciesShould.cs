using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cells.Common.Tests.Spreadsheet.Expression
{
    [TestFixture]
    public class DependenciesShould
    {
        private static IEnumerable<TestCaseData> DependencyTestCases
        {
            get
            {
                yield return new TestCaseData("= A0")
                    .Returns(new[] { new Common.Spreadsheet.Index(0, 'A') })
                    .SetName("Simple direct reference");
                yield return new TestCaseData("= Z99")
                    .Returns(new[] { new Common.Spreadsheet.Index(99, 'Z') })
                    .SetName("Maximum direct reference");
                yield return new TestCaseData("= A0:C2")
                    .Returns(
                        new[] 
                        {
                            new Common.Spreadsheet.Index(0, 'A'),
                            new Common.Spreadsheet.Index(0, 'B'),
                            new Common.Spreadsheet.Index(0, 'C'),
                            new Common.Spreadsheet.Index(1, 'A'),
                            new Common.Spreadsheet.Index(1, 'B'),
                            new Common.Spreadsheet.Index(1, 'C'),
                            new Common.Spreadsheet.Index(2, 'A'),
                            new Common.Spreadsheet.Index(2, 'B'),
                            new Common.Spreadsheet.Index(2, 'C'),
                        })
                    .SetName("Simple range");
                yield return new TestCaseData("= SUM(A0:C2)")
                    .Returns(
                        new[]
                        {
                            new Common.Spreadsheet.Index(0, 'A'),
                            new Common.Spreadsheet.Index(0, 'B'),
                            new Common.Spreadsheet.Index(0, 'C'),
                            new Common.Spreadsheet.Index(1, 'A'),
                            new Common.Spreadsheet.Index(1, 'B'),
                            new Common.Spreadsheet.Index(1, 'C'),
                            new Common.Spreadsheet.Index(2, 'A'),
                            new Common.Spreadsheet.Index(2, 'B'),
                            new Common.Spreadsheet.Index(2, 'C'),
                        })
                    .SetName("Range within a function");
            }
        }

        [TestCaseSource(nameof(DependencyTestCases))]
        public IEnumerable<Common.Spreadsheet.Index> ReturnTheExpectedDependencies(string expression)
        {
            var subject = new Common.Spreadsheet.Expression.Dependencies();
            return subject.For(expression);
        }
    }
}
