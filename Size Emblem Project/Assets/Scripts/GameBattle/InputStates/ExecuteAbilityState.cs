using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.GameBattle.InputStates
{
    public class ExecuteAbilityState : IInputState
    {
        private readonly IGameBattle _gameBattle;
        private readonly IGameUnit _unitCasting;
        private readonly IAbility _abilityTargeting;

        private readonly int _targetX;
        private readonly int _targetY;

        public ExecuteAbilityState(IGameBattle gameBattle, IGameUnit unitCasting, IAbility abilityTargeting, int targetX, int targetY)
        {
            _gameBattle = gameBattle;
            _unitCasting = unitCasting;
            _abilityTargeting = abilityTargeting;
            _targetX = targetX;
            _targetY = targetY;
        }


        public bool IsActive { get; private set; }

        public void Activate()
        {
            IsActive = true;

            // Execute the ability
            _gameBattle.ExecuteAbility(_unitCasting, _abilityTargeting, new MapPoint(_targetX, _targetY, 1, 1));


            // Reset the input stack (TODO: Handling minor actions that only reset the input stack but still allow actions)
            _gameBattle.ResetInputStack();
        }


        public void UpdateState()
        {
            
        }


        public void Deactivate()
        {
            IsActive = false;

        }
    }
}
