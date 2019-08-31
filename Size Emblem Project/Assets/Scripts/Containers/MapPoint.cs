using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Containers
{
    public struct MapPoint
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;


        public MapPoint(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }


        public bool Intersect(MapPoint other)
        {
            if(Width == 1 && other.Width == 1 && Height == 1 && other.Height == 1)

            if ((X + Width < other.X || other.X + other.Width < X) && (Y + Height < other.Y || other.Y + other.Height < Y)) return false;
            return true;
        }
    }
}
