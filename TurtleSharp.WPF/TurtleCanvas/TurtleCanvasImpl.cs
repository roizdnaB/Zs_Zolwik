using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TurtleSharp.WPF.Helpers;

namespace TurtleSharp.WPF
{
    public partial class TurtleCanvas
    {
        private void _clear()
        {
            //Clear the Canvas
            Children.Clear();

            //Set turtle to null
            _turtleRep = null;
            _turtleRotation = 0;
            _turtleTop = 0;
            _turtleLeft = 0;
            _lineRotation = 0;
            _turtlePen = true;
            _lines = new List<Line>();
        }
        public void _placeTurtle(Turtle turtle)
        {
            _turtleRep = new Polygon();
            _lines = new List<Line>();
            _turtlePen = true;
            SetLeft(_turtleRep, 0);
            SetTop(_turtleRep, 0);

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

            _initializeTurtle(turtle);

            //Set the shape of the turtle and place it in the middle of the canvas
            //this.RelocateTurtle(turtle, 0, 0);

            //Add to the Canvas
            Children.Add(_turtleRep);
        }
        private void _initializeTurtle(Turtle turtle)
        {
            Point PointA = new Point(0, 0);
            Point PointB = new Point(4, -4);
            Point PointC = new Point(2, -10);
            Point PointD = new Point(8, -8);
            Point PointE = new Point(12, -12);
            Point PointF = new Point(15, -11);
            Point PointG = new Point(21, -14);
            Point PointH = new Point(21, -9);
            Point PointI = new Point(24, -8);
            Point PointJ = new Point(24, -4);
            Point PointK = new Point(28, -4);
            Point PointL = new Point(32, 0);
            Point PointM = new Point(28, 4);
            Point PointN = new Point(24, 4);
            Point PointO = new Point(24, 8);
            Point PointP = new Point(21, 9);
            Point PointR = new Point(21, 14);
            Point PointS = new Point(15, 11);
            Point PointT = new Point(12, 12);
            Point PointU = new Point(8, 8);
            Point PointW = new Point(2, 10);
            Point PointZ = new Point(4, 4);

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

            _turtleRep.RenderTransform = new RotateTransform() { CenterX = PointA.X, CenterY = PointA.Y };
        }
        public void _removeTurtle(Turtle turtle)
        {
            //Remove the turtle from the Canvas and set it to null
            Children.Remove(_turtleRep);
            _turtleRep = null;
        }
        private void _toggleTurtleVisibility(Turtle turtle)
        {
            if (_turtleRep != null)
            {
                if (!turtle.IsVisible)
                    this.Children.Remove(_turtleRep);
                else
                    this.Children.Add(_turtleRep);
            }
        }
        private void _toggleTurtlePen(Turtle turtle) => _turtlePen = turtle.PenActive;
        private void _turtleBackward(Turtle turtle, double distance)
        {
            //Just reverse the distance value and call the forward method (smart!!!)
            TurtleForward(turtle, -distance);
            TurtleRotate(turtle, _turtleRotation - 180);
        }
        private async Task _turtleForwardAsync(Turtle turtle, double distance)
        {
            if (_turtleRep != null)
            {
                //Get the starting position of turtle (tail)
                double lineStartX = _turtleLeft;
                double lineStartY = _turtleTop;

                //Get the endpoint of the jurney
                double lineEndX = lineStartX + distance;
                double lineEndY = lineStartY;

                //Rotate the end point in the same way as the turtle
                double trueLineEndX = this.newPointX(lineEndX, lineStartX, lineEndY, lineStartY,
                    Math.Round(Math.Sin(_lineRotation), 4), Math.Round(Math.Cos(_lineRotation), 4));
                double trueLineEndY = this.newPointY(lineEndX, lineStartX, lineEndY, lineStartY,
                    Math.Round(Math.Sin(_lineRotation), 4), Math.Round(Math.Cos(_lineRotation), 4));

                //Create a line
                Line line = new Line();
                line.StrokeThickness = _brushSize;
                line.Stroke = _brushColor;

                line.X1 = lineStartX;
                line.Y1 = lineStartY;
                line.X2 = lineStartX;
                line.Y2 = lineStartY;

                var sb = new Storyboard();
                var animateX = new DoubleAnimation(lineStartX, trueLineEndX, new Duration(new TimeSpan(0, 0, 1)));
                var animateY = new DoubleAnimation(lineStartY, trueLineEndY, new Duration(new TimeSpan(0, 0, 1)));

                Storyboard.SetTarget(animateX, line);
                Storyboard.SetTarget(animateY, line);
                Storyboard.SetTargetProperty(animateX, new PropertyPath("(Line.X2)"));
                Storyboard.SetTargetProperty(animateY, new PropertyPath("(Line.Y2)"));

                sb.Children.Add(animateX);
                sb.Children.Add(animateY);
                //Show the line
                if (_turtlePen)
                {
                    this.Children.Add(line);
                    _lines.Add(line);
                }
                //Set a new position for turtle
                await _relocateTurtleAsync(turtle, trueLineEndX, trueLineEndY, sb);

                //AnimationQueue.Enqueue(sb, line);
            }
        }
        private void _turtleChangeBrush(Turtle turtle, string color)
        {
            //TODO: Change ifs to enums or something
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
        private void _turtleChangeBrushSize(Turtle turtle, double size) => _brushSize = size;//Method relocating the turtle with new tail's point
        private async Task _relocateTurtleAsync(Turtle turtle, double newCenterX, double newCenterY, Storyboard storyboard = null)
        {
            //Set the center point (The coords of the tail)
            var xCenter = newCenterX;
            var yCenter = newCenterY;

            var sb = storyboard ?? new Storyboard();
            var animationX = new DoubleAnimation(_turtleLeft, xCenter, new Duration(new TimeSpan(0, 0, 1)));
            var animationY = new DoubleAnimation(_turtleTop, yCenter, new Duration(new TimeSpan(0, 0, 1)));

            Storyboard.SetTarget(animationX, _turtleRep);
            Storyboard.SetTarget(animationY, _turtleRep);
            Storyboard.SetTargetProperty(animationX, new PropertyPath("(Canvas.Left)"));
            Storyboard.SetTargetProperty(animationY, new PropertyPath("(Canvas.Top)"));

            sb.Children.Add(animationX);
            sb.Children.Add(animationY);

            //AnimationQueue.Enqueue(sb);

            await sb.BeginAsync(CancellationToken.GetValueOrDefault());

            _turtleLeft = xCenter;
            _turtleTop = yCenter;

            //Rotate the turtle
            //this.TurtleRotate(turtle, 0);
        }
    }
}
