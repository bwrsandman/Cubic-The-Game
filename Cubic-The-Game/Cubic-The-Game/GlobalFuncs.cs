using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Cubic_The_Game
{
    class GlobalFuncs
    {
        
        /// <summary>
        /// //checks whether a point is inside of a 2D, 4-sided polygon
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static bool PointInPolygonCollision2D(Vector2 point, Vector2[] polygon)
        {

            //First, make a bounding box around the polygon, and test to see if the point is inside this
            //box
            //determine the min/max
            float minX = polygon[0].X, maxX=0, minY=polygon[0].Y, maxY=0;
            for (int i = 0; i < polygon.Length; i++)
            {
                if (polygon[i].X > maxX)
                    maxX = polygon[i].X;
                if (polygon[i].X < minX)
                    minX = polygon[i].X;
                if (polygon[i].Y > maxY)
                    maxY = polygon[i].Y;
                if (polygon[i].Y < minY)
                    minY = polygon[i].Y;
            }

            //return false if not within the box
            if (!(point.X >= minX && point.X <= maxX &&
                point.Y >= minY && point.Y <= maxY))
                return false;

            
            //{
            //    int i, j, count = 0;
            //    for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            //    {
            //        if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
            //         (point.X < (polygon[j].X - polygon[i].X) * (point.X - polygon[i].X) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
            //            count++;
            //    }
            //    if ((count & 1) == 1)
            //        return true;
            //    else
            //        return false;
         //   }
            //if it is within the box, we have to cast a line from here to the outside, and count the number
            //of intersections
            Line[] lines = new Line[4]; //assume a 4-sided polygon
            lines[0] = new Line(polygon[0].X, polygon[0].Y, polygon[1].X, polygon[1].Y);
            lines[1] = new Line(polygon[1].X, polygon[1].Y, polygon[3].X, polygon[3].Y);
            lines[2] = new Line(polygon[3].X, polygon[3].Y, polygon[2].X, polygon[2].Y);
            lines[3] = new Line(polygon[2].X, polygon[2].Y, polygon[0].X, polygon[0].Y);


            //make a horizontal line from point to outside of bounding box (to the right)
            float e = (maxX - minX)/25; //make sure the line goes well outside the polygon
            Line lineOut = null;
            //make the point go to the right if the point is more to the left, 
            //make it go to the left if the point is more to the right
            if(point.X < minX + (maxX-minX)/2)
                lineOut = new Line(point.X, point.Y, maxX + e, point.Y);
            else
                lineOut = new Line(point.X, point.Y, minX - e, point.Y);


            
            //count the number of intersections
            int numIntersections = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (AreIntersecting(lineOut, lines[i]))
                    numIntersections++;
            }
            //if there is an odd number of intersections (the last binary digit is 1), then the point is inside the polygon
            if ((numIntersections & 1) == 1)
                return true;

            return false;
        }

        //Tells us if two lines are intersecting
        private static bool AreIntersecting(Line line1, Line line2)
        {
          //put line equations into parametric form: ax + by = d1

 //------What this function does is find the point of intersection of 2 lines.
          //  x = (b2d1 - b1d2)/(a1b2 - a2b1)
         //   y = (a1d2 - a2d1)/(a1b2 - a2b1)
         //by cramer's rule, there are three possibilities:

        //- There is one solution. In this case, the denominators will be nonzero.
        //- There are no solutions. This indicates the lines are parallel (do not intersect). The denominators will be zero.
        //- There will be an infinite number of solutions. This is the case when the two lines are coincident. The denominators will be zero.

            //if there is one solution, we find it, and check to see if this solution resides within 
            //the bounds of the two lines (the solution assumes the lines stretch out to infinity)
            double a1 = line1.Y2 - line1.Y1;
            double b1 = line1.X2 - line1.X1;
            double d1 = (a1 * line1.X1) + (b1 * line1.Y1); 

            double a2 = line2.Y2 - line2.Y1;
            double b2 = line2.X2 - line2.X1;
            double d2 = (a2 * line2.X1) + (b2 * line2.Y1);

            double denom = ((a1 * b2) - (a2 * b1));
            if (denom == 0)
                return false;
            double x = ((b2 * d1) - (b1 * d2)) / denom ;
            double y = ((a1 * d2) - (a2 * d1)) / denom;

            //find out whether the solution resides within the bounds
            //find the min and max X & Y
            double minX = line1.X1 < line1.X2 ? line1.X1 : line1.X2; 
     //       minX = line
            double minY = line1.Y1 < line1.Y2 ? line1.Y1 : line1.Y2;
            double maxX = line1.X1 > line1.X2 ? line1.X1 : line1.X2;
            double maxY = line1.Y1 > line1.Y2 ? line1.Y1 : line1.Y2;

            double marg = (maxX - minX) / 1000; //margin of error

            if (!(x + marg >= minX && x - marg <= maxX && y + marg >= minY && y - marg <= maxY))
                return false;

            //do it again for line 2
            minX = line2.X1 < line2.X2 ? line2.X1 : line2.X2;
            minY = line2.Y1 < line2.Y2 ? line2.Y1 : line2.Y2;
            maxX = line2.X1 > line2.X2 ? line2.X1 : line2.X2;
            maxY = line2.Y1 > line2.Y2 ? line2.Y1 : line2.Y2;

            if (!(x >= minX && x <= maxX && y >= minY && y <= maxY))
                return false;

            return true;
        }
    }
}
