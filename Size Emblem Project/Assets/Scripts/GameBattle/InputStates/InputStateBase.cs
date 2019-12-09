using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.GameBattle.InputStates
{
    public abstract class InputStateBase : IInputState
    {
        public bool IsActive { get; set; }

        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public abstract void UpdateState();
    }
}
