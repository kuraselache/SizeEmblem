using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Constants
{
    [Flags]
    public enum DirectionFlags
    {
        None  = 0b_0000,
        North = 0b_0001,
        East  = 0b_0010,
        South = 0b_0100,
        West  = 0b_1000
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
