using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Interfaces.GameScenes
{
    public interface IGameSceneBattle
    {
        IReadOnlyList<IGameUnit> ImmediateActionQueue { get; }

        Faction CurrentPhase { get; }


        void StartBattle();

        void StartTurn();
        void EndTurn();

        void StartPhase(Faction faction);
        void EndPhase();

        void StartImmediateAction();
        void EndImmediateAction();


        bool CanUnitAct(IGameUnit unit);

        //bool IsTurnComplete();

        //bool RoundTick();
        //bool TurnTick();
    }
}
