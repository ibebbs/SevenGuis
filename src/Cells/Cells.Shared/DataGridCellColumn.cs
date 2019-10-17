using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;

namespace Cells
{
    public class DataGridCellColumn : DataGridTemplateColumn
    {
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
