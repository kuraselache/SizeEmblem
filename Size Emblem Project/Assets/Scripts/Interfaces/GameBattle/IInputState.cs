using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Interfaces.GameBattle
{
    public interface IInputState
    {
        bool IsActive { get; set; }

        /// <summary>
        /// The state has started
        /// </summary>
        void BeginState();

        /// <summary>
        /// Update this state, call this once each update-frame.
        /// </summary>
        void UpdateState();

        /// <summary>
        /// Execute this when another state is taking the place of this state, but this state is still in the state machine
        /// </summary>
        /// <param name="nextState"></param>
        void AdvanceState(IInputState nextState);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextState"></param>
        void DisposeState(IInputState nextState);

    }
}
