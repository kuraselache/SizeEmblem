using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Constants
{
    public enum SizeCategory
    {
        ExtraSmall, // Special flag, possibly for civilian units or shrunk Small/1x1 units (but should remain 1x1)
        Small, // 1x1 units
        Medium, // 2x2 units
        Large, // 3x3 units
        Gigantic, // 4x4+ units
        GiganticSuper, // Special size category
    }
}
