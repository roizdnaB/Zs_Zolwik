using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class TurtleCanvas : Canvas, ITurtlePresentation
    {
        public void Clear()
        {
            Debug.WriteLine("Cleared");
        }

        public void PlaceTurtle(Turtle turtle)
        {
            // wyświetl grafikę żółwia na tym canvasie
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
