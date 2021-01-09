namespace TurtleSharp
{
    public interface ITurtlePresentation
    {
        void PlaceTurtle(Turtle turtle);

        void RemoveTurtle(Turtle turtle);

        void TurtleForward(Turtle turtle, double distance);

        void TurtleBackward(Turtle turtle, double distance);

        void TurtleRotate(Turtle turtle, double degrees);

        void TurtleReset(Turtle turtle);

        void TurtleCurve(Turtle turtle, double radius, double length);

        void ToggleTurtleVisibility(Turtle turtle);

        void TurtleChangeBrush(Turtle turtle, string color);

        void TurtleChangeBrushSize(Turtle turtle, double size);

        void Clear();
    }
}