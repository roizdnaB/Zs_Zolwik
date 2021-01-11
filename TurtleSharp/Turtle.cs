using System;
using System.Drawing;

namespace TurtleSharp
{
    public class Turtle
    {
        private ITurtlePresentation Presentation;
        private bool _isVisible = true;
        private bool _penActive = true;
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
        public bool PenActive
        {
            get => _penActive;
            set
            {
                if (value == _penActive) return;
                else
                {
                    _penActive = value;
                    Presentation?.ToggleTurtlePen(this);
                }
            }
        }


        public Color PenColor = Color.Black; //Brush?
        public double PenSize = 1;

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

        public void ChangeBrushSize(double size) => Presentation?.TurtleChangeBrushSize(this, size);

        public void Forward(double distance) => Presentation?.TurtleForward(this, distance);

        public void Backward(double distance) => Presentation?.TurtleBackward(this, distance);

        public void Rotate(double degrees) => Presentation?.TurtleRotate(this, degrees);

        public void Curve(double radius, double length) => Presentation?.TurtleCurve(this, radius, length);

        public void Circle(double radius) => Presentation?.TurtleCurve(this, radius, 2 * Math.PI * radius);

        public void Show() => IsVisible = true;
        public void Hide() => IsVisible = false;

        public void PenUp() => PenActive = false;
        public void PenDown() => PenActive = true;

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