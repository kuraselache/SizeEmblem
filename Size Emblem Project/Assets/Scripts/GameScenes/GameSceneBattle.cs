using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameScenes;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SizeEmblem.Scripts.GameScenes
{
    public class GameSceneBattle : MonoBehaviour, IGameSceneBattle
    {

        
        // Simple state tracker. If we're busy then we're doing something, like an animation
        public bool IsBusy { get; private set; }

        // 
        public bool IsActive { get; private set; }

        public int TurnCount { get; set; }



        public List<Faction> Factions = new List<Faction>();
        private IEnumerator<Faction> _factionsEnumerator;

        public Dictionary<Faction, List<IGameUnit>> BattleUnits = new Dictionary<Faction, List<IGameUnit>>();


        public Faction CurrentPhase { get; set; } = Faction.UndefinedFaction;


        private readonly Queue<IGameUnit> _immediateActionQueue = new Queue<IGameUnit>();
        public IReadOnlyList<IGameUnit> ImmediateActionQueue { get { return _immediateActionQueue.ToList().AsReadOnly(); } }


        public void StartBattle()
        {
            // Battles start on turn 1
            TurnCount = 1;

            IsActive = true;

            // Tell all units that battle has started and to do any initialization
            BattleUnits.Values.SelectMany(x => x).ToList().ForEach(x => x.ProcessBattleStart());

            // Start the current turn
            StartTurn();
        }

        public void EndBattle()
        {
            IsActive = false;

            Debug.Log("Battle is now over!");
        }


        public void StartTurn()
        {
            // Tell all units the next turn is starting
            BattleUnits.Values.SelectMany(x => x).ToList().ForEach(x => x.ProcessTurnStart());

            // Get a new factions enumerator for this turn and move it to the first item
            _factionsEnumerator = GetNextFactionEnumerator();
            _factionsEnumerator.MoveNext();

            // Start the phase of the first faction (most likley player)
            StartPhase(_factionsEnumerator.Current);
        }

        /// <summary>
        /// End the current turn of combat. This will tell all units to run their own turn-shift 
        /// </summary>
        public void EndTurn()
        {
            // Increment the battle turn counter
            TurnCount++;

            // Then start the next turn, this will also set the next phase
            StartTurn();
        }


        public void StartPhase(Faction faction)
        {
            CurrentPhase = faction;

            // Reset the actions consumed for all units of this phase
            BattleUnits[faction].ForEach(x => x.ProcessPhaseStart());

            Debug.Log(string.Format("Current phase is: {0} on turn {1}", CurrentPhase, TurnCount));
        }


        /// <summary>
        /// End the current phase by telling all units to process any inter-phase information
        /// </summary>
        public void EndPhase()
        {
            // Tell all current faction units to do post-phase work
            BattleUnits[CurrentPhase].ForEach(x => x.ProcessPhaseEnd());

            // Start the phase of the next faction
            if(_factionsEnumerator.MoveNext())
            {
                StartPhase(_factionsEnumerator.Current);
            }
            else
            {
                // If we're out of factions then we want to end the turn
                EndTurn();
            }
        }

        
        public IEnumerator<Faction> GetNextFactionEnumerator()
        {
            var factionIndex = 0;
            while(factionIndex < Factions.Count)
            {
                var faction = Factions[factionIndex];
                if (BattleUnits[faction].Any(x => x.IsActive)) yield return faction;
                factionIndex++;
            }
        }


        public void StartImmediateAction()
        {

        }


        public void EndImmediateAction()
        {

        }


        public bool CanUnitAct(IGameUnit unit)
        {
            // Sanity check
            if (unit == null) return false;
            // If we're doing something then don't let anybody act
            if (IsBusy || !IsActive) return false;

            // If we're in an immediate action phase then this is entirely determined by the immediate action queue
            if (_immediateActionQueue.Any())
            {
                // If the unit next in the immediate action queue then they can act, otherwise they can't
                return _immediateActionQueue.Peek() == unit;
            }
            // Faction check: 
            if (unit.UnitFaction == CurrentPhase) return true;

            return false;
        }


        /// <summary>
        /// Check if the player is enabled at this time
        /// </summary>
        /// <returns></returns>
        public bool IsPlayerEnabledPhase()
        {
            // While this is busy the player can't act
            if (IsBusy || !IsActive) return false;

            // TODO: State checks for other stuff (cutscenes)

            // We're in a state that the player has control based on whose action/phase it is

            // Check if the immediate action queue. If it's a player unit then they're up!
            if (_immediateActionQueue.Any() && IsPlayerUnit(_immediateActionQueue.Peek())) return true;

            // Fallthrough: If the current faction is a player faction then they have control!
            return IsPlayerFaction(CurrentPhase);
        }


        public bool IsPlayerFaction(Faction faction)
        {
            // Ideally this should only be true for the player faction, but for now all factions are player factions
            return true;
        }

        public bool IsPlayerUnit(IGameUnit unit)
        {
            return IsPlayerFaction(unit.UnitFaction);
        }

        #region Loading

        // Current game map reference
        private IGameMap _gameMap;

        public void FindGameMap()
        {
            if(_gameMap == null)
            {
                var foundMap = Component.FindObjectOfType<SizeEmblem.Scripts.GameMap.GameMap>();
                _gameMap = foundMap;
                // If the map has been loaded then we can immediately process it. Otherwise we want to wait until it's loaded
                if(_gameMap.IsLoaded)
                {
                    LoadMap(_gameMap);
                }
                else
                {
                    _gameMap.Loaded += GameMapLoaded;
                }
                
            }
        }

        private void GameMapLoaded(IGameMap map, System.EventArgs e)
        {
            _gameMap.Loaded -= GameMapLoaded;

            LoadMap(map);
        }


        public void LoadMap(IGameMap map)
        {
            RefreshUnits(map);

            StartBattle();
        }


        public void RefreshUnits(IGameMap mapSource)
        {
            if(mapSource.MapUnits == null)
            {
                Debug.LogError("Could not load units from GameMap. MapUnits reference was null");
                return;
            }

            // Dump our old information, if any
            foreach(var battleUnitsPair in BattleUnits)
            {
                battleUnitsPair.Value.Clear();
            }
            BattleUnits.Clear();
            Factions.Clear();
            CurrentPhase = Faction.UndefinedFaction;

            // Make sure we have data to load
            if (!mapSource.MapUnits.Any()) return;
            
            // Grab all the units from the map and sort them into factions
            foreach(var unit in mapSource.MapUnits)
            {
                if (!BattleUnits.ContainsKey(unit.UnitFaction)) BattleUnits.Add(unit.UnitFaction, new List<IGameUnit>());
                BattleUnits[unit.UnitFaction].Add(unit);
            }

            // Load all factions of units in the battle and set the current faction phase to the first one (this should always be player phase)
            Factions = BattleUnits.Keys.OrderBy(x => x).ToList();
            CurrentPhase = Factions.First();
        }


        // Start is called before the first frame update
        void Start()
        {
            FindGameMap();
        }

        #endregion


        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Z))
            {
                EndPhase();
            }
        }
    }

}