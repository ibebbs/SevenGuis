using System;
using System.Text.RegularExpressions;

namespace Cells.Common.Spreadsheet
{
    public struct Index
    {
        private static readonly Regex ReferenceRegex = new Regex(@"^(?<Column>[A-Z])(?<Row>\d{1,2})$", RegexOptions.Compiled);

        public static Index Parse(string reference)
        {
            var match = ReferenceRegex.Match(reference);

            if (match.Success)
            {
                return new Index(Convert.ToInt32(match.Groups["Row"].Value), match.Groups["Column"].Value[0]);
            }
            else
            {
                throw new ArgumentException("Invalid reference");
            }
        }

        public Index(int row, char column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }

        public char Column { get; }
    }
}
