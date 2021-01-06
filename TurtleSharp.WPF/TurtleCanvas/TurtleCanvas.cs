using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TurtleSharp.WPF
{
    public partial class TurtleCanvas : Canvas, ITurtlePresentation
    {
        private Polygon _turtleRep;

        public void Clear()
        {
            //Clear the Canvas
            this.Children.Clear();
        }

        //Place the turtle on the screen
        public void PlaceTurtle(Turtle turtle)
        {
            _turtleRep = new Polygon();
            _turtleRep.Fill = Brushes.Green;

            //Calculate the coords of the middle of the canvas
            var xCenter = 0;// (200 / 2);
            var yCenter = 0;// 200 / 2;
            /*
            //Set the shape of the turtle
            Point Point1 = new Point(xCenter - 10, yCenter - 10);
            Point Point2 = new Point(xCenter + 10, yCenter - 10);
            Point Point3 = new Point(xCenter + 10, yCenter + 10);
            Point Point4 = new Point(xCenter - 10, yCenter + 10);

            //Set the collection of points and add all points to the collection
            PointCollection myPointCollection = new PointCollection();
            myPointCollection.Add(Point1);
            myPointCollection.Add(Point2);
            myPointCollection.Add(Point3);
            myPointCollection.Add(Point4);
            */
            Point PointA = new Point(xCenter - 12, yCenter);
            Point PointB = new Point(xCenter - 8, yCenter - 4);
            Point PointC = new Point(xCenter - 10, yCenter - 10);
            Point PointD = new Point(xCenter - 4, yCenter - 8);
            Point PointE = new Point(xCenter, yCenter - 12);
            Point PointF = new Point(xCenter + 3, yCenter - 11);
            Point PointG = new Point(xCenter + 9, yCenter - 14);
            Point PointH = new Point(xCenter + 9, yCenter - 9);
            Point PointI = new Point(xCenter + 12, yCenter - 8);
            Point PointJ = new Point(xCenter + 12, yCenter - 4);
            Point PointK = new Point(xCenter + 16, yCenter - 4);
            Point PointL = new Point(xCenter + 20, yCenter);
            Point PointM = new Point(xCenter + 16, yCenter + 4);
            Point PointN = new Point(xCenter + 12, yCenter + 4);
            Point PointO = new Point(xCenter + 12, yCenter + 8);
            Point PointP = new Point(xCenter + 9, yCenter + 9);
            Point PointR = new Point(xCenter + 9, yCenter + 14);
            Point PointS = new Point(xCenter + 3, yCenter + 11);
            Point PointT = new Point(xCenter, yCenter + 12);
            Point PointU = new Point(xCenter - 4, yCenter + 8);
            Point PointW = new Point(xCenter - 10, yCenter + 10);
            Point PointZ = new Point(xCenter - 8, yCenter + 4);

            PointCollection myPointCollection = new PointCollection();
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

            //Show the turtle
            _turtleRep.Points = myPointCollection;
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
            throw new NotImplementedException();
        }

        public void TurtleCurve(Turtle turtle, double radius, double length)
        {
            throw new NotImplementedException();
        }

        public void TurtleForward(Turtle turtle, double distance)
        {
            if (_turtleRep != null)
            {
                //Get the starting position of turtle
                var lineStartX = _turtleRep.Points[0].X;
                var lineStartY = _turtleRep.Points[0].Y + 10; // Plus 10 - a half of height of turtle

                //Cannot use the foreach bc Points are IEnumerable
                for (int i = 0; i < _turtleRep.Points.Count; i++)
                {
                    //Create a new Point and insert it in the place of old one
                    Point point = _turtleRep.Points[i];
                    point.X += distance;
                    _turtleRep.Points[i] = point;
                }

                //Get the ending position of turtle
                var lineEndX = _turtleRep.Points[0].X;
                var lineEndY = _turtleRep.Points[0].Y + 10;

                //Create a line
                Polyline polyline = new Polyline();
                polyline.StrokeThickness = turtle.PenSize;
                polyline.Stroke = Brushes.Black;
                polyline.Points.Add(new Point(lineStartX, lineStartY));
                polyline.Points.Add(new Point(lineEndX, lineEndY));

                //Show the line
                this.Children.Add(polyline);
            }
        }

        public void TurtleReset(Turtle turtle)
        {
            //Reset the position of turtle
            //TODO: Reset position

            //Delete all drawings form Canvas
            //foreach (var child in this.Children)
            //{
            //this.Children.Remove(child);
            //}
        }

        public void TurtleRotate(Turtle turtle, double degrees)
        {
            throw new NotImplementedException();
        }
    }
}