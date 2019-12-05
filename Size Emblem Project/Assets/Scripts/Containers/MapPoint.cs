using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Containers
{
    [Serializable]
    public struct MapPoint
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public int Key { get { return X * 1000 + Y; } }

        public MapPoint(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }


        public bool CollidesWith(MapPoint other)
        {
            if (X + Width - 1 < other.X || other.X + other.Width - 1 < X || Y + Height - 1 < other.Y || other.Y + other.Height - 1 < Y) return false;
            return true;
        }

        public bool SameOrigin(MapPoint other)
        {
            return X == other.X && Y == other.Y;
        }

        public MapPoint ApplyOffset(int offsetX, int offsetY)
        {
            return new MapPoint(X + offsetX, Y + offsetY, Width, Height);
        }

    }
}
