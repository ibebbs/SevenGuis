using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Reflection;
using Windows.UI.Xaml;

namespace Cells
{
    public class DataGridCellColumn : DataGridTemplateColumn
    {
        private static readonly PropertyInfo RowIndexProperty = typeof(DataGridCell).GetProperty("RowIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly PropertyInfo ColumnIndexProperty = typeof(DataGridCell).GetProperty("ColumnIndex", BindingFlags.NonPublic | BindingFlags.Instance);

        public DataGridCellColumn(char column)
        {
            Column = column;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            Common.RowViewModel rowViewModel = (Common.RowViewModel)dataItem;

            var element = base.GenerateElement(cell, dataItem);

            element.DataContext = rowViewModel[Column];

            return element;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            Common.RowViewModel rowViewModel = (Common.RowViewModel)dataItem;

            var element = base.GenerateEditingElement(cell, dataItem);

            element.DataContext = rowViewModel[Column];

            return element;
        }

        public char Column { get; }
    }
}
