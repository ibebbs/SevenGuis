using System.Windows;
using System.Windows.Controls;

namespace Cells
{
    public class DataGridCellColumn : DataGridTemplateColumn
    {
        public static readonly DependencyProperty CellProperty = DependencyProperty.RegisterAttached("Cell", typeof(object), typeof(DataGridCellColumn), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static object GetCell(DependencyObject obj)
        {
            return (object)obj.GetValue(CellProperty);
        }

        public static void SetCell(DependencyObject obj, object value)
        {
            obj.SetValue(CellProperty, value);
        }

        public DataGridCellColumn(char column)
        {
            Column = column;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            Common.RowViewModel rowViewModel = (Common.RowViewModel)dataItem;

            var element = base.GenerateElement(cell, dataItem);

            SetCell(element, rowViewModel[Column]);

            return element;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            Common.RowViewModel rowViewModel = (Common.RowViewModel)dataItem;

            var element = base.GenerateEditingElement(cell, dataItem);

            SetCell(element, rowViewModel[Column]);

            return element;
        }

        public char Column { get; }
    }
}
