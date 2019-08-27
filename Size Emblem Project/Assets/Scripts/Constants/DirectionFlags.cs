using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Constants
{
    public enum DirectionFlags
    {
        North = 0x1,
        East = 0x2,
        South = 0x4,
        West = 0x8
    }

    public static class DirectionFlagsHelper
    {
        public static bool HasDirectionFlag(int testValue, DirectionFlags directionFlag)
        {
            return (testValue & (int)directionFlag) > 0;
        }

        public static int SetDirectionFlag(int startValue, DirectionFlags directionFlag)
        {
            return startValue | (int)directionFlag;
        }

        public static int UnsetDirectionFlag(int startValue, DirectionFlags directionFlag)
        {
            return startValue ^ ~((int)directionFlag);
        }

    }
}
