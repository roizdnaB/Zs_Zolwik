using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TurtleSharp.WPF.Helpers;
using static TurtleSharp.WPF.Helpers.SynchronizedDispatcher;

namespace TurtleSharp.WPF
{
    public partial class TurtleCanvas : Canvas, ITurtlePresentation
    {
        private Polygon _turtleRep = null;
        private double _turtleRotation = 0;
        private double _lineRotation = 0;
        private Brush _brushColor = Brushes.Black;
        private double _brushSize = 1;
        private double _turtleLeft = 0;
        private double _turtleTop = 0;
        private bool _turtlePen = true;
        private CancellationToken? _cancellationToken;
        private SynchronizedDispatcher _queueDispatcher;

        public CancellationToken? CancellationToken {
            get => _cancellationToken;
            set
            {
                _cancellationToken = value;
                if (_queueDispatcher != null) _queueDispatcher.CancellationToken = value;
            }
        }

        //The list required to save file as CVG
        private List<Line> _lines;

        public void Clear() => Dispatcher.Invoke(() =>
        {
            //Clear the Canvas
            this.Children.Clear();

            //Set turtle to null
            _turtleRep = null;
            _turtleRotation = 0;
            _turtleTop = 0;
            _turtleLeft = 0;
            _lineRotation = 0;
            _turtlePen = true;
            _lines = new List<Line>();
        });

        //Place the turtle on the screen
        public void PlaceTurtle(Turtle turtle) => Dispatcher.Invoke(() =>
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

            this.InitializeTurtle(turtle);

            //Set the shape of the turtle and place it in the middle of the canvas
            //this.RelocateTurtle(turtle, 0, 0);

            //Add to the Canvas
            this.Children.Add(_turtleRep);
        });

        private void InitializeTurtle(Turtle turtle)
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

        public void RemoveTurtle(Turtle turtle) => RunOnUISynchronized(() =>
        {
            //Remove the turtle from the Canvas and set it to null
            this.Children.Remove(_turtleRep);
            _turtleRep = null;
        });

        public void ToggleTurtleVisibility(Turtle turtle) => RunOnUISynchronized(() =>
        {
            if (_turtleRep != null) { 
            if (!turtle.IsVisible)
                this.Children.Remove(_turtleRep);
            else
                this.Children.Add(_turtleRep);
            }
        });

        public void ToggleTurtlePen(Turtle turtle) => RunOnUISynchronized(() =>
        {
            _turtlePen = turtle.PenActive;
        });

        public void TurtleBackward(Turtle turtle, double distance) => RunOnUISynchronized(() =>
        {
            //Just reverse the distance value and call the forward method (smart!!!)
            TurtleForward(turtle, -distance);
            TurtleRotate(turtle, _turtleRotation - 180);
        });

        public void TurtleCurve(Turtle turtle, double radius, double length)
        {
            throw new NotImplementedException();
        }

        public void TurtleForward(Turtle turtle, double distance) => RunOnUISynchronized(async arg =>
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
                await RelocateTurtleAsync(turtle, trueLineEndX, trueLineEndY, sb);

                //AnimationQueue.Enqueue(sb, line);
            }

            arg.NotifyCompletion();
        });

        public void TurtleReset(Turtle turtle)
        {
            //Save the current color and size, and apply it after placing turtle againt
            var color = _brushColor;
            var size = _brushSize;
            var lines = _lines;
            var turtlePen = _turtlePen;

            //Delete the turtle from Canvas and add a new one
            this.RemoveTurtle(turtle);
            this.PlaceTurtle(turtle);

            _brushColor = color;
            _brushSize = size;
            _lines = lines;
            _turtlePen = turtlePen;
        }

        
        public void TurtleRotate(Turtle turtle, double degrees) => RunOnUISynchronized(async arg =>
        {
            if (degrees == 0) return;
            //Convert degrees to radiants
            double angle = (Math.PI * -degrees) / 180.0;

            _lineRotation += angle;

            var turtleTransform = _turtleRep.RenderTransform as RotateTransform;

            var rotationAnimation = new DoubleAnimation(_turtleRotation, _turtleRotation - degrees, new Duration(new TimeSpan(0, 0, 1)));
            
            await turtleTransform.BeginAnimationAsync(RotateTransform.AngleProperty, rotationAnimation, CancellationToken.GetValueOrDefault());

            _turtleRotation -= degrees;

            arg.NotifyCompletion();
        });

        public void TurtleChangeBrush(Turtle turtle, string color) => RunOnUISynchronized(() =>
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
       });

        public void TurtleChangeBrushSize(Turtle turtle, double size)
        {
            _brushSize = size;
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
        private async Task RelocateTurtleAsync(Turtle turtle, double newCenterX, double newCenterY, Storyboard storyboard = null)
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

        private void RunOnUI(Action action) => Dispatcher.Invoke(() =>
        {
            if (CancellationToken != null)
            {
                var ct = (CancellationToken)CancellationToken;

                if (!ct.IsCancellationRequested)
                    Dispatcher.InvokeAsync(action, DispatcherPriority.Normal, ct).Wait();

                ct.ThrowIfCancellationRequested();
            }
            else
            {
                action.Invoke();
            }
        });

        private void RunOnUISynchronized(Action action)
        {
            if (_queueDispatcher is null)
            {
                _queueDispatcher = new SynchronizedDispatcher(Dispatcher);
                _queueDispatcher.CancellationToken = CancellationToken;
            }
            if (!CancellationToken?.IsCancellationRequested ?? true)
            {
                _queueDispatcher.Invoke(action);
            }
            else
            {
                CancellationToken?.ThrowIfCancellationRequested();
            }
        }
        private void RunOnUISynchronized(Action<ActionCompletedNotifier> action)
        {
            if (_queueDispatcher is null)
            {
                _queueDispatcher = new SynchronizedDispatcher(Dispatcher);
                _queueDispatcher.CancellationToken = CancellationToken;
            }
            if (!CancellationToken?.IsCancellationRequested ?? true)
            {
                _queueDispatcher.Invoke(action);
            }
            else
            {
                CancellationToken?.ThrowIfCancellationRequested();
            }
        }
    }
}