﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Interfaces.Managers
{
    public interface IRNGManager
    {
        ulong GetRange(ulong min, ulong max);
    }
}
