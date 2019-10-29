using SizeEmblem.Assets.Scripts.Constants;
using SizeEmblem.Assets.Scripts.UI;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Events.GameMap;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameScenes;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SizeEmblem.Scripts.GameScenes
{
    public class GameSceneBattle : MonoBehaviour, IGameSceneBattle
    {
        #region Unity Dependencies


        public SelectedUnitSummaryWindow uiSelectedUnitSummary;
        public SelectedUnitAbilitiesWindow selectedUnitAbilitiesWindow;

        #endregion

        public BattleSceneState State { get; private set; } = BattleSceneState.Initialize;

        public void UpdateState(BattleSceneState newState)
        {
            if (State == newState) return;
            State = newState;

            if(State == BattleSceneState.PlayerPhase)
            {
                _gameMap.IsCursorEnabled = true;
                uiSelectedUnitSummary.IsEnabled = true;
            }
            else
            {
                _gameMap.IsCursorEnabled = false;
                uiSelectedUnitSummary.IsEnabled = false;
            }
        }


        // Simple state tracker. If we're busy then we're doing something, like an animation
        public bool IsBusy { get { return _busyCounter > 0; } }
        private int _busyCounter;

        // 
        public bool IsActive { get; private set; }


        #region Turn & Phase Control


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
            UpdateState(BattleSceneState.BattleComplete);
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
            // Update our state to turn change
            UpdateState(BattleSceneState.TurnChange);

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

            Debug.Log(String.Format("Current phase is: {0} on turn {1}", CurrentPhase, TurnCount));

            // Insert animation co-routine here
            StartCoroutine(StartPhaseCoroutine());
        }

        public IEnumerator StartPhaseCoroutine()
        {
            yield return new WaitForSeconds(1);

            Debug.Log("PHASE START!");

            StartPhaseComplete();
        }

        public void StartPhaseComplete()
        {
            UpdateState(IsPlayerEnabledPhase() ? BattleSceneState.PlayerPhase : BattleSceneState.AITurn);
        }


        /// <summary>
        /// End the current phase by telling all units to process any inter-phase information
        /// </summary>
        public void EndPhase()
        {
            // Update our state to phase change
            UpdateState(BattleSceneState.PhaseChange);

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

        #endregion


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


        #region UI


        private void GameMap_HoverUnitChanged(IGameMap gameMap, UnitSelectedEventArgs e)
        {
            if(uiSelectedUnitSummary.IsEnabled)
            {
                uiSelectedUnitSummary.SelectedUnit = e.Unit;
            }
        }


        private void GameMap_PlayerSelectedUnit(IGameMap map, UnitSelectedEventArgs e)
        {
            SelectedUnit(e.Unit);
        }


        private void GameMap_PlayerSelectedRoute(IGameMap map, RouteSelectedEventArgs e)
        {
            MoveSelectedUnit(e.Route);
        }

        private void Map_UnitMoveCompleted(object sender, EventArgs e)
        {
            MoveUnitCompleted();
        }

        private void GameMap_BackButton(object sender, EventArgs e)
        {
            GoBackState();
        }

        #endregion

        private IGameUnit _selectedUnit;

        public void SelectedUnit(IGameUnit unit)
        {
            // If selected a new unit when none was selected
            if(_selectedUnit == null && unit != null)
            {
                _selectedUnit = unit;
                _gameMap.ShowUnitMovementRange(_selectedUnit);
                return;
            }


        }

        private bool _isMovingUnit;
        private bool _movingCursorEnabledState;

        public void MoveSelectedUnit(IGameMapMovementRoute route)
        {
            if (_selectedUnit == null) return;

            MoveUnit(_selectedUnit, route);
            _selectedUnit = null;
        }

        public void MoveUnit(IGameUnit unit, IGameMapMovementRoute route)
        {
            // Input sanity checks
            if(unit == null || route == null)
            {
                Debug.Log("Invalid state for SelectedRoute");
                return;
            }
            if(_isMovingUnit)
            {
                Debug.Log("Duplicate attempt to move unit");
                return;
            }

            // Make sure the passed unit can act if it is the player's phase state. Otherwise we're being controlled by a command or AI
            if (State == BattleSceneState.PlayerPhase && !CanUnitAct(unit)) return;

            // Mark ourselves as busy and disable inputs for the duration of the move
            _busyCounter++;
            _isMovingUnit = true;
            _movingCursorEnabledState = _gameMap.IsCursorEnabled;
            _gameMap.IsCursorEnabled = false;

            // Move the unit
            
            _gameMap.MoveUnit(_selectedUnit, route);
        }

        public void MoveUnitCompleted()
        {
            if (!_isMovingUnit) return;
            _isMovingUnit = false;
            _busyCounter--;
            _gameMap.IsCursorEnabled = _movingCursorEnabledState;
            _selectedUnit = null;
        }


        
        public void GoBackState()
        {
            if(_selectedUnit != null)
            {
                _gameMap.ClearMovementOverlay();
                _selectedUnit = null;
            }
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

        private void GameMapLoaded(IGameMap map, EventArgs e)
        {
            _gameMap.Loaded -= GameMapLoaded;

            LoadMap(map);
        }


        public void LoadMap(IGameMap map)
        {
            UpdateState(BattleSceneState.Initialize);
            // Load the current map data
            RefreshUnits(map);

            // Hook up events
            map.HoverUnitChanged += GameMap_HoverUnitChanged;
            map.PlayerSelectedUnit += GameMap_PlayerSelectedUnit;
            map.PlayerSelectedRoute += GameMap_PlayerSelectedRoute;
            map.UnitMoveCompleted += Map_UnitMoveCompleted;

            map.BackButton += GameMap_BackButton;

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