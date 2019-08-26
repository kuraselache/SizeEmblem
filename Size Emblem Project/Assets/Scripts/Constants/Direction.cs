using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Scripts.Constants
{
    public enum Direction
    {
        None,
        North,
        East,
        South,
        West
    }

    public static class DirectionHelper
    {
        public static void ApplyDirection(Direction direction, ref int x, ref int y)
        {
            switch(direction)
            {
                case Direction.East: x++; break;
                case Direction.West: x--; break;
                case Direction.North: y++; break;
                case Direction.South: y--; break;
            }
        }

        public static bool DirectionVertical(Direction direction)
        {
            return (direction == Direction.North || direction == Direction.South);
        }

        public static bool DirectionHorizontal(Direction direction)
        {
            return (direction == Direction.East || direction == Direction.West);
        }

        public static Vector3 GetDirectionVector(Direction direction)
        {
            switch (direction)
            {
                case Direction.East:  return Vector3.right;
                case Direction.West:  return Vector3.left;
                case Direction.North: return Vector3.up;
                case Direction.South: return Vector3.down;
                default: return Vector3.zero;
            }

        }
    }
}
