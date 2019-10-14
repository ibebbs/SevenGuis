namespace CircleDrawer.Domain.Command
{
    public class ChangeDiameter : ICommand
    {
        public ChangeDiameter(Circle circle, int diameter)
        {
            Circle = circle;
            Diameter = diameter;
        }

        public Circle Circle { get; }
        public int Diameter { get; }
    }
}
