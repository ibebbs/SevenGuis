using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Cells.Common
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private static readonly Random Random = new Random();

        private readonly Crux.Bus _bus;
        private readonly Spreadsheet.Sheet _sheet;

        public MainPageViewModel(IScheduler scheduler)
        {
            _bus = new Crux.Bus();
            _sheet = new Spreadsheet.Sheet(_bus);

            Rows = Enumerable
                .Range(0, 99)
                .Select(row => new RowViewModel(_bus, row, Enumerable
                    .Range('A', 26)
                    .Select(columnIndex => new CellViewModel(_bus, row, (char)columnIndex, scheduler))
                    .ToArray()))
                .ToArray();
        }

        public void Activate()
        {
        }

        public void Deactivate()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<RowViewModel> Rows { get; }
    }
}
