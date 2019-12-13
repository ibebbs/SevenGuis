using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cells.Common
{
    public class RowViewModel : INotifyPropertyChanged
    {
        private readonly MVx.Observable.IBus _bus;
        private readonly IReadOnlyDictionary<char, CellViewModel> _cells;

        public RowViewModel(MVx.Observable.IBus bus, int row, CellViewModel[] cells)
        {
            _bus = bus;
            _cells = cells.ToDictionary(cell => cell.Column, cell => cell);

            Row = row;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public int Row { get; }

        public CellViewModel this[char column]
        {
            get { return _cells[column]; }
        }
    }
}
