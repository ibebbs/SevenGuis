using System.Collections.Generic;

namespace Cells.Common.Spreadsheet
{
    public class Cell
    {
        public Cell(Index index, string text, object content)
        {
            Index = index;
            Text = text;
            Content = content;

            Dependencies = Expression.Dependencies.For(text);
        }

        public Index Index { get; }
        public IEnumerable<Index> Dependencies { get; }
        public string Text { get; }
        public object Content { get; set; }
    }
}
