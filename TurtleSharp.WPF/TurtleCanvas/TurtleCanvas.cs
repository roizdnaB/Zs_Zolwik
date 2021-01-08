using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TurtleSharp.WPF
{
    public partial class TurtleCanvas : Canvas, ITurtlePresentation
    {
        private Polygon _turtleRep = null;
        private double _turtleRotation = 0;
        private double _lineRotation = 0;
        private Brush _brushColor = Brushes.Black;

        public void Clear()
        {
            //Clear the Canvas
            this.Children.Clear();

            //Set turtle to null
            _turtleRep = null;
        }

        //Place the turtle on the screen
        public void PlaceTurtle(Turtle turtle)
        {
            _turtleRep = new Polygon();
            _turtleRep.Fill = Brushes.Green;
            _turtleRep.Stroke = Brushes.Black;
            _turtleRep.StrokeThickness = 2;

            //Make shell green
            _turtleRep.FillRule = FillRule.Nonzero;

            //Set the brush color
            _brushColor = Brushes.Black;

            //Set the rotation values
            _turtleRotation = 0;
            _lineRotation = 0;

            //Set the shape of the turtle and place it in the middle of the canvas
            this.RelocateTurtle(turtle, 0, 0);

            //Add to the Canvas
            this.Children.Add(_turtleRep);
        }

        public void RemoveTurtle(Turtle turtle)
        {
            //Remove the turtle from the Canvas and set it on null
            this.Children.Remove(_turtleRep);
            _turtleRep = null;
        }

        public void ToggleTurtleVisibility(Turtle turtle)
        {
            //Just reverse the bool
            turtle.IsVisible = turtle.IsVisible;
        }

        public void TurtleBackward(Turtle turtle, double distance)
        {
            //Just reverse the distance value and call the forward method (smart!!!)
            this.TurtleForward(turtle, -distance);
            this.TurtleRotate(turtle, _turtleRotation - 180);
        }

        public void TurtleCurve(Turtle turtle, double radius, double length)
        {
            throw new NotImplementedException();
        }

        public void TurtleForward(Turtle turtle, double distance)
        {
            if (_turtleRep != null)
            {
                //Get the starting position of turtle (tail)
                double lineStartX = _turtleRep.Points[0].X;
                double lineStartY = _turtleRep.Points[0].Y;

                //Get the endpoint of the jurney
                double lineEndX = _turtleRep.Points[0].X + distance;
                double lineEndY = _turtleRep.Points[0].Y;

                //Rotate the end point in the same way as the turtle
                double trueLineEndX = this.newPointX(lineEndX, lineStartX, lineEndY, lineStartY,
                    Math.Round(Math.Sin(_lineRotation), 4), Math.Round(Math.Cos(_lineRotation), 4));
                double trueLineEndY = this.newPointY(lineEndX, lineStartX, lineEndY, lineStartY,
                    Math.Round(Math.Sin(_lineRotation), 4), Math.Round(Math.Cos(_lineRotation), 4));

                //Create a line
                Polyline polyline = new Polyline();
                polyline.StrokeThickness = turtle.PenSize;
                polyline.Stroke = _brushColor;
                polyline.Points.Add(new Point(lineStartX, lineStartY));
                polyline.Points.Add(new Point(trueLineEndX, trueLineEndY));

                //Set a new position for turtle
                this.RelocateTurtle(turtle, trueLineEndX, trueLineEndY);

                //Show the line
                this.Children.Add(polyline);
            }
        }

        public void TurtleReset(Turtle turtle)
        {
            //Save the current color and apply it after placing turtle againt
            var color = _brushColor;

            //Delete the turtle from Canvas and add a new one
            this.RemoveTurtle(turtle);
            this.PlaceTurtle(turtle);

            _brushColor = color;
        }

        public void TurtleRotate(Turtle turtle, double degrees)
        {
            //Convert degrees to radiants
            double angle = (Math.PI * -degrees) / 180.0;

            //Set the rotation variables
            //If degrees isn't 0 - set the angle, otherwise set the turtle's rotation to line rotation
            if (degrees != 0)
                _turtleRotation = angle;
            else
                _turtleRotation = _lineRotation;

            _lineRotation += angle;

            //Save the sin and cos values
            double sinVal = Math.Round(Math.Sin(_turtleRotation), 4);
            double cosVal = Math.Round(Math.Cos(_turtleRotation), 4);

            //Get the tail coords of the turtle
            double xTail = _turtleRep.Points[0].X;
            double yTail = _turtleRep.Points[0].Y;

            for (int i = 0; i < _turtleRep.Points.Count; i++)
            {
                //Create a new Point and insert it in the place of old one
                Point point = _turtleRep.Points[i];
                point.X = this.newPointX(_turtleRep.Points[i].X, xTail, _turtleRep.Points[i].Y, yTail, sinVal, cosVal);
                point.Y = this.newPointY(_turtleRep.Points[i].X, xTail, _turtleRep.Points[i].Y, yTail, sinVal, cosVal);
                _turtleRep.Points[i] = point;
            }
        }

        public void TurtleChangeBrush(Turtle turtle, string color)
        {
            if (color == "Red")
                _brushColor = Brushes.Red;
            else if (color == "Green")
                _brushColor = Brushes.Green;
            else if (color == "Yellow")
                _brushColor = Brushes.Yellow;
            else if (color == "Blue")
                _brushColor = Brushes.Blue;
            else if (color == "Purple")
                _brushColor = Brushes.Purple;
            else if (color == "Black")
                _brushColor = Brushes.Black;
            else
                return;
        }

        //Method calculating a new point X - helper method for Rotate and moving forward/backward
        private double newPointX(double x, double xs, double y, double ys, double sin, double cos)
        {
            return ((x - xs) * cos) - ((y - ys) * sin) + xs;
        }

        //Method calculating a new point Y - helper method for Rotate and moving forward/backward
        private double newPointY(double x, double xs, double y, double ys, double sin, double cos)
        {
            return ((x - xs) * sin) + ((y - ys) * cos) + ys;
        }

        //Method relocating the turtle with new tail's point
        private void RelocateTurtle(Turtle turtle, double newCenterX, double newCenterY)
        {
            //Set the center point (The coords of the tail)
            var xCenter = newCenterX;
            var yCenter = newCenterY;

            //Set the shape of the turtle
            Point PointA = new Point(xCenter, yCenter);
            Point PointB = new Point(xCenter + 4, yCenter - 4);
            Point PointC = new Point(xCenter + 2, yCenter - 10);
            Point PointD = new Point(xCenter + 8, yCenter - 8);
            Point PointE = new Point(xCenter + 12, yCenter - 12);
            Point PointF = new Point(xCenter + 15, yCenter - 11);
            Point PointG = new Point(xCenter + 21, yCenter - 14);
            Point PointH = new Point(xCenter + 21, yCenter - 9);
            Point PointI = new Point(xCenter + 24, yCenter - 8);
            Point PointJ = new Point(xCenter + 24, yCenter - 4);
            Point PointK = new Point(xCenter + 28, yCenter - 4);
            Point PointL = new Point(xCenter + 32, yCenter);
            Point PointM = new Point(xCenter + 28, yCenter + 4);
            Point PointN = new Point(xCenter + 24, yCenter + 4);
            Point PointO = new Point(xCenter + 24, yCenter + 8);
            Point PointP = new Point(xCenter + 21, yCenter + 9);
            Point PointR = new Point(xCenter + 21, yCenter + 14);
            Point PointS = new Point(xCenter + 15, yCenter + 11);
            Point PointT = new Point(xCenter + 12, yCenter + 12);
            Point PointU = new Point(xCenter + 8, yCenter + 8);
            Point PointW = new Point(xCenter + 2, yCenter + 10);
            Point PointZ = new Point(xCenter + 4, yCenter + 4);

            //Set the collection of points and add all points to the collection
            PointCollection myPointCollection = new PointCollection();

            //Turtle
            myPointCollection.Add(PointA);
            myPointCollection.Add(PointB);
            myPointCollection.Add(PointC);
            myPointCollection.Add(PointD);
            myPointCollection.Add(PointE);
            myPointCollection.Add(PointF);
            myPointCollection.Add(PointG);
            myPointCollection.Add(PointH);
            myPointCollection.Add(PointI);
            myPointCollection.Add(PointJ);
            myPointCollection.Add(PointK);
            myPointCollection.Add(PointL);
            myPointCollection.Add(PointM);
            myPointCollection.Add(PointN);
            myPointCollection.Add(PointO);
            myPointCollection.Add(PointP);
            myPointCollection.Add(PointR);
            myPointCollection.Add(PointS);
            myPointCollection.Add(PointT);
            myPointCollection.Add(PointU);
            myPointCollection.Add(PointW);
            myPointCollection.Add(PointZ);

            //Shell
            myPointCollection.Add(PointA);
            myPointCollection.Add(PointE);
            myPointCollection.Add(PointI);
            myPointCollection.Add(PointO);
            myPointCollection.Add(PointT);

            //Show the turtle
            _turtleRep.Points = myPointCollection;

            //Rotate the turtle
            this.TurtleRotate(turtle, 0);
        }
    }
}