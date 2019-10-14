using System;
using System.Reactive.Linq;

namespace CircleDrawer.Domain
{
    public static class Logic
    {
        public static readonly int InitialDiameter = 25;

        public static IObservable<IState> Observable(IObservable<ICommand> commands)
        {
            return commands.Scan(State.Empty, State.Apply);
        }
    }
}
