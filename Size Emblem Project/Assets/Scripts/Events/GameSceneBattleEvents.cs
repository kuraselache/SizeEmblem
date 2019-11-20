using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using SizeEmblem.Scripts.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Events
{
    public delegate void BattleSceneTurnChangedEventHandler(IGameBattle battleScene, TurnChangedEventArgs e);

    public class TurnChangedEventArgs : EventArgs
    {
        public readonly int TurnNumber;

        public TurnChangedEventArgs(int turnNumber)
        {
            TurnNumber = turnNumber;
        }
    }


    public delegate void BattleScenePhaseChangedEventHandler(IGameBattle battleScene, PhaseChangedEventArgs e);

    public class PhaseChangedEventArgs : EventArgs
    {
        public Faction FactionPhase;

        public PhaseChangedEventArgs(Faction factionPhase)
        {
            FactionPhase = factionPhase;
        }
    }

}
