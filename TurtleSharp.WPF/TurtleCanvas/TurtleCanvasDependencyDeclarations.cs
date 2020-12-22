using System;
using System.Windows;

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
        }

        private static void OnTurtlePresentationHookChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var canvas = (TurtleCanvas)dependencyObject;
            canvas._turtlePresentationHook = args.NewValue as ITurtlePresentation;
        }

        #endregion TurtlePresentationHookDefinition
    }
}