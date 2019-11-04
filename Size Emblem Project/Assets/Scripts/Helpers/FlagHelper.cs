using SizeEmblem.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Helpers
{
    public static class FlagHelper
    {

        public static bool HasFlag(int testValue, int flag)
        {
            return (testValue & flag) > 0;
        }

        public static int SetFlag(int startValue, int directionFlag)
        {
            return startValue | directionFlag;
        }

        public static int UnsetFlag(int startValue, int directionFlag)
        {
            return startValue ^ ~(directionFlag);
        }

        public static int SetFlags(int[] flags)
        {
            var value = 0;
            flags.ForEach(x => value |= x);
            return value;
        }
    }
}
