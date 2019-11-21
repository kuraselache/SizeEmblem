using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Interfaces.GameBattle
{
    public interface IInputState
    {
        bool IsActive { get; }

        /// <summary>
        /// The state has started
        /// </summary>
        void Activate();

        /// <summary>
        /// Update this state, call this once each update-frame.
        /// </summary>
        void UpdateState();

        /// <summary>
        /// 
        /// </summary>
        void Deactivate();

    }
}
