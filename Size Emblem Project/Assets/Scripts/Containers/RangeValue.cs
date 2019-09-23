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

    }

    [Serializable]
    public class RangeValueULong : RangeValue<ulong>
    {
    }
}
