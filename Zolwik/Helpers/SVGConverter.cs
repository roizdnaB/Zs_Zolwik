using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Shapes;

namespace Zolwik.Helpers
{
    public static class SVGConverter
    {
        static readonly CultureInfo Culture = CultureInfo.CreateSpecificCulture("en-US");

        public static string GetSVGCode(List<Line> objects)
        {
            //Define the head of the SVG file
            //TODO: Add the correct size of the file
            string svgHead = "" +
                "<svg version=\"1.1\" " +
                "baseProfile=\"full\" " +
                "viewBox=\"-250 -250 500 500\" " +
                "xmlns=\"http://www.w3.org/2000/svg\" > ";

            //Define the end of the SVG file
            string svgEnd = "</svg>";

            //Define the whole document
            string doc = svgHead;

            //Add all lines to the svg (delete the aplha value from color)
            foreach (var obj in objects)
                doc += getSvgLine(obj.X1, obj.Y1, obj.X2, obj.Y2, "#" + 
                    string.Join(string.Empty, obj.Stroke.ToString().Skip(3)), obj.StrokeThickness);

            //Add the end of the file
            doc += svgEnd;

            //Return code
            return doc;
        }

        //Get the code of the line in SVG
        private static string getSvgLine(double x1, double y1, double x2, double y2, string color, double size)
        {
            return "" +
                $"<line x1=\"{x1.ToString(Culture)}\" y1=\"{y1.ToString(Culture)}\" " +
                $"x2=\"{x2.ToString(Culture)}\" y2=\"{y2.ToString(Culture)}\" " +
                $"stroke=\"{color}\" stroke-width=\"{size.ToString(Culture)}\" />";
        }
    }
}