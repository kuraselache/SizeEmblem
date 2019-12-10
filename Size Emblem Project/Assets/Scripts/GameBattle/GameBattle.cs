using SizeEmblem.Assets.Scripts.UI;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using SizeEmblem.Assets.Scripts.GameBattle.InputStates.Factory;
using SizeEmblem.Assets.Scripts.GameBattle;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Assets.Scripts.Calculators;
using SizeEmblem.Assets.Scripts.Containers;
using SizeEmblem.Assets.Scripts.Interfaces.GameUnits;
using SizeEmblem.Scripts.Extensions;

namespace SizeEmblem.Scripts.GameScenes
{
    public class GameBattle : MonoBehaviour, IGameBattle
    {
        #region Unity Dependencies


        public UnitSummaryWindow uiUnitSummaryWindow;
        public UnitDetailsWindow unitDetailsWindow;
        public UnitAbilitiesWindow unitAbilitiesWindow;
        public UnitActionWindow unitActionWindow;
        public EndPhaseWindow endPhaseWindow;


        #endregion

        public BattleStates State { get; private set; } = BattleStates.Initialize;

        public void UpdateState(BattleStates newState)
        {
            if (State == newState) return;
            State = newState;

            if(State == BattleStates.PlayerPhase)
            {
                InitializeInputStateStack();
            }
            else
            {
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
            UpdateState(BattleStates.BattleComplete);
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
            UpdateState(BattleStates.TurnChange);

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
            UpdateState(IsPlayerEnabledPhase() ? BattleStates.PlayerPhase : BattleStates.AITurn);
        }


        /// <summary>
        /// End the current phase by telling all units to process any inter-phase information
        /// </summary>
        public void EndPhase()
        {
            // We can only end phase if we're in a player or AI phase state
            if (!(State == BattleStates.PlayerPhase || State == BattleStates.AITurn)) return;

            // Clean up our input states, if any
            ClearInputStack();

            // Clear our window UI on phase change
            HideAllWindows();

            // Update our state to phase change
            UpdateState(BattleStates.PhaseChange);

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



        public bool IsUnitsPhase(IGameUnit unit)
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



        public void HideAllWindows()
        {
            uiUnitSummaryWindow.IsVisible = false;
            uiUnitSummaryWindow.IsEnabled = false;
            unitAbilitiesWindow.IsVisible = false;
            unitActionWindow.IsVisible = false;
            endPhaseWindow.IsVisible = false;

            _gameMap.ClearMovementOverlay();
            _gameMap.ClearAbilityRange();
            
        }


        public void ResetInputState()
        {
            if (State != BattleStates.PlayerPhase) return;

            InitializeInputStateStack();
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
            UpdateState(BattleStates.Initialize);
            // Load the current map data
            RefreshUnits(map);

            // Now that we have our map reference we can create our input state factory (since it has a dependency on the map)
            CreateStateFactory();

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
        public void Start()
        {
            FindGameMap();
            //SetUpUnitActionWindow();
            //SetUpSelectedUnitAbilitiesWindow();
            //SetupEndPhaseWindow();
        }



        #endregion



        #region Input State Management

        private readonly Stack<IInputState> _inputStateStack = new Stack<IInputState>();

        private IInputStateFactory _inputStateFactory;

        /// <summary>
        /// Add a new input state to the current input state stack.
        /// This will mark the current state, if any, as inactive and inform it of the new state that is taking its place.
        /// Then this will add the new state to the stack, mark it active, and call the BeginState method.
        /// </summary>
        /// <param name="nextInputState"></param>
        public void AddInputState(IInputState nextInputState)
        {
            if (nextInputState == null) throw new ArgumentNullException(nameof(nextInputState), "Given new state passed to GameBattle AddInputState method is null");

            // Deactivate any existing state on the stack
            if(_inputStateStack.Any())
            {
                var lastState = _inputStateStack.Peek();
                lastState.Deactivate();
            }

            // Add the new state to the stack and 
            _inputStateStack.Push(nextInputState);
            nextInputState.Activate();
        }

        public void SetState(IInputState newInputState)
        {
            ClearInputStack();
            AddInputState(newInputState);
        }


        /// <summary>
        /// Get the current player input state of the battle controller.
        /// </summary>
        /// <param name="currentState">The current plyaer input state of the battle controller. Null if it isn't in a state to receive player input.</param>
        /// <returns>True if a valid player input state was returned. False otherwise.</returns>
        public bool CurrentInputState(out IInputState currentState)
        {
            if (State != BattleStates.PlayerPhase || !_inputStateStack.Any())
            {
                currentState = null;
                return false;
            }

            currentState = _inputStateStack.Peek();
            return true;
        }

        public void ResetInputStack()
        {

            ClearInputStack();
            InitializeInputStateStack();
        }

        public void ClearInputStack()
        {
            while (_inputStateStack.Any())
            {
                var topState = _inputStateStack.Pop();
                if (topState.IsActive) topState.Deactivate();
                _inputStateFactory.DisposeState(topState);
            }
        }


        public void ClearTopInputState()
        {
            // Sanity check: If the input state is empty then there's nothing to remove
            if (!_inputStateStack.Any()) return;

            var topState = _inputStateStack.Pop();
            topState.Deactivate();
            _inputStateFactory.DisposeState(topState);

            var nextTopState = _inputStateStack.Any() ? _inputStateStack.Peek() : null;
            nextTopState.Activate();
        }


        private void InitializeInputStateStack()
        {
            if (_inputStateStack.Any()) return;

            AddInputState(_inputStateFactory.ResolveSelectUnitState());
        }


        private void CreateStateFactory()
        {
            if (_inputStateFactory != null) return;

            _inputStateFactory = new InputStateFactory(this, _gameMap, uiUnitSummaryWindow, unitDetailsWindow, unitActionWindow, unitAbilitiesWindow, endPhaseWindow);
        }




        #endregion



        public void ExecuteAbility(IGameUnit user, IAbility ability, MapPoint targetPoint)
        {
            var targetPoints = ability.AreaPoints.Select(x => x.ApplyOffset(targetPoint.X, targetPoint.Y));
            
            var foundTargets = _gameMap.FindAllMapObjectsInBounds(out var targets, targetPoints);
            var validTargets = AbilityTargetCalculator.FilterTargets(user, ability, targets);

            // Get a queue of abilities and their execution
            var executionQueue = CreateAbiltyQueue(user, ability, validTargets, targetPoint, _gameMap);

            // Now execute each entry in our queue
            while(executionQueue.Any())
            {
                var executionParams = executionQueue.Dequeue();
                var executionResults = new List<AbilityResultContainer>();

                // If the params doesn't have a target assigned we'll create dupes and populate the targets
                if(executionParams.Target == null)
                {
                    var newParams = executionParams.AllTargets.Select(x => new AbilityExecuteParameters(executionParams.UnitExecuting, executionParams.AbilityExecuting, x, executionParams.AllTargets, executionParams.TargetPoint, executionParams.GameMap));
                    var newParamsResults = newParams.SelectMany(x => ExecuteAbilityParameters(x)).ToList();
                    executionResults.AddRange(newParamsResults);
                }
                // Otherwise we'll just execute the ability
                else
                {
                    var paramsResults = ExecuteAbilityParameters(executionParams);
                    executionResults.AddRange(paramsResults);
                }

                // DO SOME ANIMATION HERE?

                // ALSO IF A COUNTER HAS A COST IT ISN"T DEDUCTED EVER

            }

            // Now that we executed the ability we need to consume the cost of the ability
            ConsumeAbilityForUnit(user, ability);

            // Check for defeated units
            validTargets.OfType<IGameUnit>().Where(x => x.IsDead()).ForEach(x => RemoveUnit(x));
            if (user.IsDead()) RemoveUnit(user);

        }

        public Queue<AbilityExecuteParameters> CreateAbiltyQueue(IGameUnit user, IAbility abilityToExecute, IEnumerable<IGameMapObject> targets, MapPoint targetPoint, IGameMap gameMap)
        {
            // Create our action queue
            var abilityExecuteQueue = new Queue<AbilityExecuteParameters>();

            // For now we'll only target game units from the targets list
            var gameUnitTargets = targets.OfType<IGameUnit>().ToList();
            // Sanity check: If we _don't_ have any game units then 
            if (!gameUnitTargets.Any()) return abilityExecuteQueue;


            // Create a counter-attack object for each target
            var targetCounterAttacks = gameUnitTargets.Where(x => abilityToExecute.CanBeCountered).Where(x => x.UnitFaction != user.UnitFaction).Select(counteringUnit =>
            {
                var counterAbility = GetAutoAttackAbility(counteringUnit, user);

                return new
                {
                    CounterUnit = counteringUnit,
                    CounterAbility = counterAbility,
                    CounterRepeat = counterAbility != null ? DamageCalculator.RepeatCount(counterAbility, counteringUnit, user) : 0,
                };
            }).Where(x => x.CounterAbility != null).ToList();

            // We need the number of repeats total. This'll govern how many loops we do of filling our queue
            var userRepeatCount = gameUnitTargets.Select(target => DamageCalculator.RepeatCount(abilityToExecute, user, target)).Max();
            var maxRepeatCount = targetCounterAttacks.Any() ? Math.Max(userRepeatCount, targetCounterAttacks.Max(x => x.CounterRepeat)) : userRepeatCount;

            // Pregen list
            var counterTargetsList = new List<IGameMapObject> { user };

            for(var i = 0; i < maxRepeatCount; i++)
            {
                // Add the user's action to the queue if they have this many repeats
                if(i < userRepeatCount)
                    abilityExecuteQueue.Enqueue(new AbilityExecuteParameters(user, abilityToExecute, null, targets, targetPoint, gameMap));
                // Then add all the counter attacks!
                foreach(var counterAttack in targetCounterAttacks.Where(x => i < x.CounterRepeat))
                {
                    abilityExecuteQueue.Enqueue(new AbilityExecuteParameters(counterAttack.CounterUnit, counterAttack.CounterAbility, user, counterTargetsList, user.MapPoint, gameMap));
                }
            }

            return abilityExecuteQueue;
        }

        public IAbility GetAutoAttackAbility(IGameUnit user, IGameUnit target)
        {
            if (user.Abilities == null || !user.Abilities.Any(x => x.CanCounterAttack)) return null;

            // Get all possible counter abilities for this user
            var counterAbilities = user.Abilities.Where(ability => ability.CanCounterAttack && user.CanUseAbility(ability) && AbilityRangeCalculator.CanAbilityTargetUnit(user, target, ability)).ToList();
            if (!counterAbilities.Any()) return null;
            // And pick the strongest one!
            var strongestAbility = counterAbilities.Select(ability =>
            { 
                var abilityParams = new AbilityExecuteParameters(user, ability, target, new List<IGameMapObject> { target }, target.MapPoint, _gameMap);
                var repeatCount = DamageCalculator.RepeatCount(ability, user, target);
                var results = ability.AbilityEffects.Select(x => x.PreviewResults(abilityParams)).Sum(x => x.BaseDamage * x.HitRate) * repeatCount;
                return new { Ability = ability, DamageScore = results };
            }).MaxBy(x => x.DamageScore).Ability;

            return strongestAbility;
        }


        public IEnumerable<AbilityResultContainer> ExecuteAbilityParameters(AbilityExecuteParameters abilityExecuteParameters)
        {
            // Check to see if the user of this ability can act. If they can't then we can't execute this action (ie they died during execution, or are disabled)
            // Also check if the target is dead. Don't attack if they're dead
            if (!abilityExecuteParameters.UnitExecuting.CanAct() || (abilityExecuteParameters.Target is IGameUnit && (abilityExecuteParameters.Target as IGameUnit).IsDead())) return Enumerable.Empty<AbilityResultContainer>();

            // Execute each ability effect and return the results containers
            return abilityExecuteParameters.AbilityExecuting.AbilityEffects.Select(x => ExecuteAbilityEffect(abilityExecuteParameters, x)).ToList();

        }


        public AbilityResultContainer ExecuteAbilityEffect(AbilityExecuteParameters abilityExecuteParameters, IAbilityEffect effect)
        {
            // I'll have to break this up and change all this later, but for now I'm tired and just want something to work for now
            var effectResults = effect.CreateResults(abilityExecuteParameters);
            effect.ExecuteEffect(abilityExecuteParameters, effectResults);
            return effectResults;
        }


        public void RemoveUnit(IGameUnit unit)
        {
            _gameMap.RemoveMapUnit(unit);
        }


        public void ConsumeAbilityForUnit(IGameUnit unit, IAbility ability)
        {
            unit.ConsumeAbilityCost(ability);
        }


        public void Update()
        {
            if(State == BattleStates.PlayerPhase && _inputStateStack.Any())
            {
                _inputStateStack.Peek().UpdateState();
            }
        }
    }

}