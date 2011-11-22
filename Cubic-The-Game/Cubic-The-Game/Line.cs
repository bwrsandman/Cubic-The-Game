using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Microsoft.Xna.Framework;

namespace Cubic_The_Game
{
    class Line
    {
 
        public Line(float x1, float y1, float x2, float y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public float X1{get; set;}
        public float X2 { get; set; }
        public float Y1 { get; set; }
        public float Y2 { get; set; }
    }
}
