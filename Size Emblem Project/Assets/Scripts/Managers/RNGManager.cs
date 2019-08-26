using Assets.Scripts.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Managers
{
    public class RNGManager : IRNGManager
    {
        public int seed = 12345;

        private Random _rngBank1;

        public RNGManager()
        {
            _rngBank1 = new Random((int)DateTime.Now.Ticks);
        }


        public ulong GetRange(ulong min, ulong max)
        {
            var range = max - min;
            var rng = _rngBank1.NextDouble();
            var rngValue = (ulong)(range * rng) + min;
            return rngValue;
        }
    }
}
