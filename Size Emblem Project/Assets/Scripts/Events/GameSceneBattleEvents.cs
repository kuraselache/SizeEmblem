using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.GameScenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Events
{
    public delegate void BattleSceneTurnChangedEventHandler(IGameSceneBattle battleScene, TurnChangedEventArgs e);

    public class TurnChangedEventArgs : EventArgs
    {
        public readonly int TurnNumber;

        public TurnChangedEventArgs(int turnNumber)
        {
            TurnNumber = turnNumber;
        }
    }


    public delegate void BattleScenePhaseChangedEventHandler(IGameSceneBattle battleScene, PhaseChangedEventArgs e);

    public class PhaseChangedEventArgs : EventArgs
    {
        public Faction FactionPhase;

        public PhaseChangedEventArgs(Faction factionPhase)
        {
            FactionPhase = factionPhase;
        }
    }

}
