using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace TurtleSharp.WPF
{
    public partial class TurtleCanvas
    {
        #region TurtlePresentationHookDefinition

        public static readonly DependencyProperty TurtlePresentationHookProperty =
                DependencyProperty.Register(
                    "TurtlePresentationHook",
                    typeof(ITurtlePresentation),
                    typeof(TurtleCanvas),
                    new FrameworkPropertyMetadata(null, OnTurtlePresentationHookChanged)
                    );

        private ITurtlePresentation _turtlePresentationHook
        {
            get { return (ITurtlePresentation)GetValue(TurtlePresentationHookProperty); }
            set { SetValue(TurtlePresentationHookProperty, this); }
        }

        public ITurtlePresentation TurtlePresentationHook
        {
            get { return (ITurtlePresentation)GetValue(TurtlePresentationHookProperty); }
            set { throw new Exception("Can't set the Turtle Presentation Hook of a TurtleCanvas, it's readonly."); }
        }

        static TurtleCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TurtleCanvas), new FrameworkPropertyMetadata(typeof(TurtleCanvas)));
        }

        public TurtleCanvas() : base()
        {
            SetValue(TurtlePresentationHookProperty, this);
            //SetValue(ClipToBoundsProperty, true);

            //The commented part below should swap the Y coordinates
            //but instead it gliches the canvas out totally :(
            RenderTransform = new ScaleTransform();// { ScaleX = 1, ScaleY = -1 };

            MouseWheel += MouseWheelCallback;
        }

        private static void OnTurtlePresentationHookChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var canvas = (TurtleCanvas)dependencyObject;
            canvas._turtlePresentationHook = args.NewValue as ITurtlePresentation;
        }

        #endregion TurtlePresentationHookDefinition

        private void MouseWheelCallback(object sender, MouseWheelEventArgs e)
        {
            var rt = RenderTransform as ScaleTransform;

            var zoomDelta = e.Delta > 0 ? 1.25 : 1.0 / 1.25;

            rt.ScaleX *= zoomDelta;
            rt.ScaleY *= zoomDelta;

            //This part should make the canvas center at the mouse position when zooming in and out.
            //It's kinda wanky but it's better than nothing.
            //And certainly better than using `this` instead of `Parent`.
            //That was a total mess.

            var mousePosition = Mouse.GetPosition(Parent as IInputElement);

            rt.CenterX = mousePosition.X;
            rt.CenterY = mousePosition.Y;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Point middle = new Point(arrangeSize.Width / 2, arrangeSize.Height / 2);

            foreach (UIElement element in InternalChildren)
            {
                if (element == null)
                {
                    continue;
                }

                double left = GetLeft(element);
                double top = GetTop(element);

                double x = !double.IsNaN(left) ? left : 0;
                double y = !double.IsNaN(top) ? top : 0;

                element.Arrange(new Rect(new Point(middle.X + x, middle.Y + y), element.DesiredSize));
            }
            return arrangeSize;
        }
    }
}