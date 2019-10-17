using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Cells.Common.Spreadsheet
{
    public class Sheet : IDisposable
    {
        private readonly Crux.IBus _bus;
        private readonly Dictionary<Index, Cell> _cells;
        private readonly Expression.Evaluator _expressionEvaluator;
        private readonly Expression.Dependencies _expressionDependencies;

        private IDisposable _behaviours;

        public Sheet(Crux.IBus bus)
        {
            _bus = bus;
            _cells = new Dictionary<Index, Cell>();
            _expressionEvaluator = new Expression.Evaluator(Lookup);
            _expressionDependencies = new Expression.Dependencies();

            _behaviours = new CompositeDisposable(
                ShouldRecalculateDependentCellsWhenTextChanges()
            );
        }

        public void Dispose()
        {
            if (_behaviours != null)
            {
                _behaviours.Dispose();
                _behaviours = null;
            }
        }

        private object Lookup(Index index)
        {
            return _cells.TryGetValue(index, out Cell value)
                ? value.Content
                : null;
        }

        private IEnumerable<Cell> FindDependencies(Cell current, Index[] traversed)
        {
            yield return current;

            traversed = traversed.Concat(new[] { current.Index }).ToArray();

            foreach (var cell in _cells.Values)
            {
                if (cell.Dependencies.Contains(current.Index))
                {
                    if (traversed.Contains(cell.Index))
                    {
                        // Circular loop
                        throw new InvalidOperationException();
                    }

                    foreach (var dependency in FindDependencies(cell, traversed))
                    {
                        yield return dependency;
                    }
                }
            }
        }

        private IEnumerable<Cell> CollateDependencies(Event.TextChanged textChanged)
        {
            var index = new Index(textChanged.Row, textChanged.Column);
            var cell = new Cell(index, textChanged.Text, null);

            _cells[index] = cell;

            return FindDependencies(cell, new Index[0]);
        }

        private Cell RecalculateCell(Cell cell)
        {
            var content = string.IsNullOrWhiteSpace(cell.Text)
                ? null
                : cell.Text.IsExpression()
                ? _expressionEvaluator.Evaluate(cell.Text)
                : Content.Evaluator.Evaluate(cell.Text);

            cell.Content = content;

            return cell;
        }

        private IDisposable ShouldRecalculateDependentCellsWhenTextChanges()
        {
            return _bus
                .GetEvent<Event.TextChanged>()
                .Select(@event => new { Event = @event, Dependencies = CollateDependencies(@event) })
                .SelectMany(tuple => tuple.Dependencies
                    .Select(RecalculateCell)
                    .Select(cell => new Event.ContentChanged(Guid.NewGuid(), tuple.Event.Id, tuple.Event.Id) { Row = cell.Index.Row, Column = cell.Index.Column, Content = cell.Content }))
                .Subscribe(_bus.Publish);
        }
    }
}
