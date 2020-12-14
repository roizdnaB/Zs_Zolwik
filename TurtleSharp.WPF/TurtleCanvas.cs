﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TurtleSharp.WPF
{
    public class TurtleCanvas : Canvas, ITurtlePresentation
    {
        static TurtleCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TurtleCanvas), new FrameworkPropertyMetadata(typeof(TurtleCanvas)));
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void PlaceTurtle(Turtle turtle)
        {
            throw new NotImplementedException();
        }

        public void RemoveTurtle(Turtle turtle)
        {
            throw new NotImplementedException();
        }

        public void ToggleTurtleVisibility(Turtle turtle)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void TurtleReset(Turtle turtle)
        {
            throw new NotImplementedException();
        }

        public void TurtleRotate(Turtle turtle, double degrees)
        {
            throw new NotImplementedException();
        }
    }
}
