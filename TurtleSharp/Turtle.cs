using System;
using System.Drawing;

namespace TurtleSharp
{
    public class Turtle
    {
        private ITurtlePresentation Presentation;
        private bool _isVisible = true;
        public bool IsPlacedOnSomething => Presentation != null;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                else
                {
                    _isVisible = value;
                    Presentation?.ToggleTurtleVisibility(this);
                }
            }
        }

        public Color PenColor = Color.Black; //Brush?
        public double PenSize = 1;
        public bool PenActive = true;

        public void Place(ITurtlePresentation presentation)
        {
            Presentation?.RemoveTurtle(this);

            Presentation = presentation;
            presentation.PlaceTurtle(this);
        }

        public void TakeOff()
        {
            if (IsPlacedOnSomething)
            {
                Presentation.RemoveTurtle(this);
                Presentation = null;
            }
        }

        public void ChangeBrush(string color) => Presentation?.TurtleChangeBrush(this, color);

        public void Forward(double distance) => Presentation?.TurtleForward(this, distance);

        public void Backward(double distance) => Presentation?.TurtleBackward(this, distance);

        public void Rotate(double degrees) => Presentation?.TurtleRotate(this, degrees);

        public void Curve(double radius, double length) => Presentation?.TurtleCurve(this, radius, length);

        public void Circle(double radius) => Presentation?.TurtleCurve(this, radius, 2 * Math.PI * radius);

        public void Square(double sideLength)
        {
            if (IsPlacedOnSomething)
            {
                Forward(sideLength);
                Rotate(90);
                Forward(sideLength);
                Rotate(90);
                Forward(sideLength);
                Rotate(90);
                Forward(sideLength);
            }
        }

        public void Triangle(double sideLength)
        {
            if (IsPlacedOnSomething)
            {
                Forward(sideLength);
                Rotate(120);
                Forward(sideLength);
                Rotate(120);
                Forward(sideLength);
            }
        }

        public void GoHome() => Presentation?.TurtleReset(this);
    }
}