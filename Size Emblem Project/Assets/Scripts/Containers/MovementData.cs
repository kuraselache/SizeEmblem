using SizeEmblem.Scripts.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Scripts.Containers
{
    [Serializable]
    public class MovementData
    {
        public MovementType movementType;
        

        public bool Passable = true;

        [Range(0.1f, 10)]
        public float MovementCost = 1;
    }
}
