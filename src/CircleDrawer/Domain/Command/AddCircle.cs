using System.Drawing;

namespace CircleDrawer.Domain.Command
{
    public class AddCircle : ICommand
    {
        public AddCircle(Circle circle)
        {
            Circle = circle;
        }
        public Circle Circle { get; }
    }
}
