using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleDrawer.Domain
{
    public interface IState
    {
        IEnumerable<Circle> Circles { get; }

        Circle Selected { get; }

        bool CanUndo { get; }

        bool CanRedo { get; }
    }

    internal class State : IState
    {
        public static readonly State Empty = new State(Enumerable.Empty<Circle>(), null, Enumerable.Empty<ICommand>(), Enumerable.Empty<ICommand>());

        private static State Apply(State state, Command.AddCircle command)
        {
            return new State(
                state.Circles.Concat(new[] { command.Circle }).ToArray(),
                command.Circle,
                new[] { command }.Concat(state.UndoStack).ToArray(),
                Enumerable.Empty<ICommand>()
            );
        }

        private static State Undo(State state, Command.AddCircle command)
        {
            var circle = state.Circles.Where(c => c.Equals(command.Circle)).First();

            return new State(
                state.Circles.Except(new[] { circle }).ToArray(),
                null,
                state.UndoStack.Except(new[] { command }).ToArray(),
                new[] { command }.Concat(state.RedoStack).ToArray()
            );
        }

        private static State Redo(State state, Command.AddCircle command)
        {
            return new State(
                state.Circles.Concat(new[] { command.Circle }).ToArray(),
                command.Circle,
                new[] { command }.Concat(state.UndoStack).ToArray(),
                state.RedoStack.Except(new [] {command }).ToArray()
            );
        }

        private static State Apply(State state, Command.ChangeDiameter command)
        {
            return new State(
                state.Circles,
                command.Circle,
                new[] { command }.Concat(state.UndoStack).ToArray(),
                Enumerable.Empty<ICommand>()
            );
        }

        private static State Undo(State state, Command.ChangeDiameter command)
        {
            var previousDiameter = state.UndoStack
                .OfType<Command.ChangeDiameter>()
                .Where(c => !c.Equals(command) && c.Circle.Equals(command.Circle))
                .Select(c => (int?)c.Diameter)
                .FirstOrDefault() ?? Logic.InitialDiameter;

            var circle = state.Circles
                .Where(c => c.Equals(command.Circle))
                .First();

            circle.Diameter = previousDiameter;

            return new State(
                state.Circles,
                command.Circle,
                state.UndoStack.Except(new[] { command }).ToArray(),
                new[] { command }.Concat(state.RedoStack).ToArray()
            );
        }

        private static State Redo(State state, Command.ChangeDiameter command)
        {
            var circle = state.Circles.Where(c => c.Equals(command.Circle)).First();
            circle.Diameter = command.Diameter;

            return new State(
                state.Circles,
                command.Circle,
                new[] { command }.Concat(state.UndoStack).ToArray(),
                state.RedoStack.Except(new[] { command }).ToArray()
            );
        }

        private static State Apply(State state, Command.Undo command)
        {
            var commandToUndo = state.UndoStack.First();

            switch (commandToUndo)
            {
                case Command.AddCircle addCircle: return Undo(state, addCircle);
                case Command.ChangeDiameter changeDiameter: return Undo(state, changeDiameter);
                default: throw new InvalidOperationException("Unknown command type");
            }
        }

        private static State Apply(State state, Command.Redo command)
        {
            var commandToUndo = state.RedoStack.First();

            switch (commandToUndo)
            {
                case Command.AddCircle addCircle: return Redo(state, addCircle);
                case Command.ChangeDiameter changeDiameter: return Redo(state, changeDiameter);
                default: throw new InvalidOperationException("Unknown command type");
            }
        }

        public static State Apply(State state, ICommand command)
        {
            switch (command)
            {
                case Command.AddCircle addCircle: return Apply(state, addCircle);
                case Command.ChangeDiameter changeDiameter: return Apply(state, changeDiameter);
                case Command.Undo undo: return Apply(state, undo);
                case Command.Redo redo: return Apply(state, redo);
                default: throw new InvalidOperationException("Unknown command type");
            }
        }

        public State(IEnumerable<Circle> circles, Circle selected, IEnumerable<ICommand> undoStack, IEnumerable<ICommand> redoStack)
        {
            Circles = circles;
            Selected = selected;
            UndoStack = undoStack;
            RedoStack = redoStack;
        }

        public IEnumerable<Circle> Circles { get; }

        public Circle Selected { get; }

        public IEnumerable<ICommand> UndoStack { get; }

        public IEnumerable<ICommand> RedoStack { get; }

        public bool CanUndo => UndoStack.Any();

        public bool CanRedo => RedoStack.Any();
    }
}
