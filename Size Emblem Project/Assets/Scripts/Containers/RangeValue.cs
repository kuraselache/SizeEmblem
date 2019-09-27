using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Containers
{
    
    public class RangeValue<T>
    {
        public T minValue;
        public T maxValue;


        public RangeValue()
        {

        }

        public RangeValue(T min, T max)
        {
            minValue = min;
            maxValue = max;
        }
    }

    [Serializable]
    public class RangeValueULong : RangeValue<ulong>
    {
    }
}
