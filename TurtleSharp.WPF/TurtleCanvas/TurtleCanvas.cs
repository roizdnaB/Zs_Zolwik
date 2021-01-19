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
        private CancellationToken _cancellationToken;
        private SynchronizedDispatcher _queueDispatcher;
        private SynchronizedDispatcher QueueDispatcher
        {
            get
            {
                if (_queueDispatcher is null)
                {
                    _queueDispatcher = new SynchronizedDispatcher(Dispatcher);
                    _queueDispatcher.CancellationToken = CancellationToken;
                    _queueDispatcher.QueueEmptied += delegate {
                        IsBusy = false;
                    };
                }
                return _queueDispatcher;
            }
        }

        //The list required to save file as CVG
        private List<Line> _lines;

        public CancellationToken CancellationToken {
            get => _cancellationToken;
            set
            {
                _cancellationToken = value;
                _cancellationToken.Register(() =>
                {
                    _clear();
                    Dispatcher.Invoke(() =>
                    {
                        IsBusy = false;
                    });
                });
                if (QueueDispatcher != null) QueueDispatcher.CancellationToken = value;
            }
        }

        public void Clear() => RunOnUISynchronized(_clear);
        //Place the turtle on the screen
        public void PlaceTurtle(Turtle turtle) => RunOnUISynchronized(() => _placeTurtle(turtle));
        public void RemoveTurtle(Turtle turtle) => RunOnUISynchronized(() => _removeTurtle(turtle));
        public void ToggleTurtleVisibility(Turtle turtle) => RunOnUISynchronized(() => _toggleTurtleVisibility(turtle));
        public void ToggleTurtlePen(Turtle turtle) => RunOnUISynchronized(() => _toggleTurtlePen(turtle));
        public void TurtleBackward(Turtle turtle, double distance) => RunOnUISynchronized(() => _turtleBackward(turtle, distance));
        public void TurtleCurve(Turtle turtle, double radius, double length)
        {
            throw new NotImplementedException();
        }
        public void TurtleForward(Turtle turtle, double distance) => RunOnUISynchronized(async arg =>
        {
            await _turtleForwardAsync(turtle, distance);
            arg.NotifyCompletion();
        });
        public void TurtleReset(Turtle turtle) => RunOnUISynchronized(() => _turtleReset(turtle));
        private void _turtleReset(Turtle turtle)
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
            await _turtleRotateAsync(turtle, degrees);
            arg.NotifyCompletion();
        });

        private async Task _turtleRotateAsync(Turtle turtle, double degrees)
        {
            if (degrees == 0) return;
            //Convert degrees to radiants
            double angle = (Math.PI * -degrees) / 180.0;

            _lineRotation += angle;

            var turtleTransform = _turtleRep.RenderTransform as RotateTransform;

            var rotationAnimation = new DoubleAnimation(_turtleRotation, _turtleRotation - degrees, new Duration(new TimeSpan(0, 0, 1)));

            await turtleTransform.BeginAnimationAsync(RotateTransform.AngleProperty, rotationAnimation, CancellationToken);

            _turtleRotation -= degrees;
        }

        public void TurtleChangeBrush(Turtle turtle, string color) => RunOnUISynchronized(() => _turtleChangeBrush(turtle, color));
        public void TurtleChangeBrushSize(Turtle turtle, double size) => RunOnUISynchronized(() => _turtleChangeBrushSize(turtle, size));

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
            if (!CancellationToken.IsCancellationRequested)
            {
                QueueDispatcher.Invoke(action);
            }
            else
            {
                CancellationToken.ThrowIfCancellationRequested();
            }
        }
        private void RunOnUISynchronized(Action<ActionCompletedNotifier> action)
        {
            if (!CancellationToken.IsCancellationRequested)
            {
                QueueDispatcher.Invoke(action);
            }
            else
            {
                CancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}