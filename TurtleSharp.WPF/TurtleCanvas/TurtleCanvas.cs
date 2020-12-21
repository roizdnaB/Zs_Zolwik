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
        private Rectangle _turtleRep;

        public void Clear()
        {
            //Clear the Canvas
            this.Children.Clear();
        }

        //Place the turtle on the screen
        public void PlaceTurtle(Turtle turtle)
        {
            //Set the repr as the Rectangle
            _turtleRep = new Rectangle();
            
            //Set the parameters
            _turtleRep.Width = 15;
            _turtleRep.Height = 15;

            //Set the color
            _turtleRep.Fill = new SolidColorBrush(Colors.Green);

            //Display the turtle
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
            turtle.IsVisible = !turtle.IsVisible;
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
