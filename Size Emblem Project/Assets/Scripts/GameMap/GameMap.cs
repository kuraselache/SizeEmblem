using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.Extensions;
using SizeEmblem.Scripts.GameMap.Factories;
using SizeEmblem.Scripts.Helpers.Comparers;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameMap.Factories;
using SizeEmblem.Scripts.Interfaces.Managers;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using SizeEmblem.Scripts.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Text;
using SizeEmblem.Scripts.Interfaces.GameScenes;
using SizeEmblem.Scripts.GameScenes;
using SizeEmblem.Scripts.Events.GameMap;
using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Assets.Scripts.UI;
using SizeEmblem.Assets.Scripts.GameUnits;
using SizeEmblem.Assets.Scripts.Interfaces.Managers;
using SizeEmblem.Assets.Scripts.Managers;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.GameMap
{

    public class GameMap : MonoBehaviour, IGameMap
    {
        #region Unity Dependencies

        private Camera _mapcamera;


        public Tilemap baseTileMap;
        public Tilemap[] baseTileMaps;

        public Tilemap objectTileMap;

        public Tilemap movementOverlay;
        public TileBase overlayTile;

        public GameMapCursor gameMapCursor;

        #endregion

        #region Code Dependencies

        private IGameSystemManager _gameSystem;
        private IInputManager _inputManager;
        private IGameMapTileGroupFactory _gameMapTileGroupFactory;

        public void GetDepenencies()
        {
            // Add DI eventually here
            _gameSystem = GameSystemManager.Instance;
            _inputManager = new UnityInputManager();
            _gameMapTileGroupFactory = new GameMapTileGroupFactory();

            FindGameSceneBattle();
        }

        private IGameSceneBattle _gameSceneBattle;

        public void FindGameSceneBattle()
        {
            if (_gameSceneBattle == null)
            {
                var sceneBattle = Component.FindObjectOfType<GameSceneBattle>();
                _gameSceneBattle = sceneBattle;
            }
        }

        #endregion


        #region Map Dimension & Origin Information

        // Width & Height dimensions of the map, only tiles in this area will be considered part of the play area
        public int gameMapWidth;
        public int GameMapWidth
        {
            get { return gameMapWidth; }
            private set
            {
                if (value == gameMapWidth) return;
                gameMapWidth = value;
            }
        }


        public int gameMapHeight;
        public int GameMapHeight
        {
            get { return gameMapHeight; }
            set
            {
                if (value == gameMapHeight) return;
                gameMapHeight = value;
            }
        }

        // Since Unity space has negative values but we don't have negative values in our map data array we need an origin or offset X,Y coordinates so we can translate Unity X,Y into our map array X,Y
        // This should be the bottom left corner of the play area
        public int gameMapOriginX;
        public int GameMapOriginX { get { return gameMapOriginX; } }

        public int gameMapOriginY;
        public int GameMapOriginY { get { return gameMapOriginY; } }

        // Number of base map layers this map has
        public int MapLayers { get { return baseTileMaps.Length; } }

        public IGameMapTileGroup[] GameMapTiles { get; private set; }


        public void RefreshGameMapTileArray()
        {
            Debug.Log("Start game map tiles refresh");
            // Create our array to hold the map data
            if (GameMapTiles == null || GameMapTiles.Length != GameMapWidth * GameMapHeight)
            {
                GameMapTiles = new GameMapTileGroup[GameMapWidth * GameMapHeight];
                for (var i = 0; i < GameMapTiles.Length; i++)
                    GameMapTiles[i] = _gameMapTileGroupFactory.Resolve(MapLayers);
            }

            Debug.Log("Array initialized, begin populating");

            // Go through each map layer and load found tiles into our tiles array
            for (var mapLayer = 0; mapLayer < MapLayers; mapLayer++)
            {
                Debug.Log(String.Format("Processing layer: {0}", mapLayer));
                LoadMapLayer(GameMapTiles, GameMapWidth, GameMapHeight, mapLayer);
            }

            Debug.Log("Population complete");
        }

        public void LoadMapLayer(IGameMapTileGroup[] gameMapTiles, int gmWidth, int gmHeight, int mapLayer)
        {
            // Get the tilemap we're working on, based on the map layer variable
            var tileMap = baseTileMaps[mapLayer];

            // Get all the map tiles on this base tile map
            var mapTiles = tileMap.GetComponentsInChildren<GameMapTile>();

            foreach (var mapTile in mapTiles)
            {
                // Convert the unity position of this tile to the tilemap's position
                var cellPosition = tileMap.WorldToCell(mapTile.transform.position);

                // Get the map X,Y for this tile's cell position
                TranslateUnityXYToMapXY(cellPosition, out var mapX, out var mapY);

                // Assign this tile it's Map X,Y values
                mapTile.MapX = mapX;
                mapTile.MapY = mapY;

                // Make sure it's in bounds of our array. If it isn't then there's no need to add it to our tiles matrix
                if (mapX < 0 || mapX >= gmWidth || mapY < 0 || mapY >= gmHeight) continue;



                var index = mapY * gmWidth + mapX;
                if (index >= gameMapTiles.Length) Debug.Log("OH SHIT");

                // The tile is in bounds of our intended map data so add it to our game map tiles array
                gameMapTiles[mapY * gmWidth + mapX].SetLayerTile(mapLayer, mapTile);
            }
        }


        public void TranslateUnityXYToMapXY(Vector3Int unityPosition, out int mapX, out int mapY)
        {
            TranslateUnityXYToMapXY(unityPosition.x, unityPosition.y, out mapX, out mapY);
        }

        public void TranslateUnityXYToMapXY(int unityX, int unityY, out int mapX, out int mapY)
        {
            var newX = unityX - GameMapOriginX;
            mapX = newX; // Mathf.Clamp(newX, 0, MapWidth - 1);
            var newY = unityY - gameMapOriginY;
            mapY = newY; // Mathf.Clamp(newY, 0, MapHeight - 1);
        }


        public Vector3Int TranslateMapXYToUnityXY(int mapX, int mapY)
        {
            var vector = new Vector3Int(mapX + GameMapOriginX, mapY + GameMapOriginY, 0);
            return vector;
        }



        //public IGameMapTileGroup GetTiles(Vector3Int worldPosition)
        //{
        //    // Convert our unity positioin to our map X,Y
        //    TranslateUnityXYToMapXY(worldPosition, out var mapX, out var mapY);

        //    // Calculate the array index for our given X,Y based on the map dimensions
        //    var index = mapY * GameMapWidth + mapX;

        //    // Return the tiles at our calculated index
        //    return GameMapTiles[index];
        //}

        #endregion


        #region Passibility & Movement Cost


        public MovementCost AreaMovementCostForUnit(IGameUnit unit, int mapX, int mapY, int areaWidth = 1, int areaHeight = 1)
        {
            // Sanity checks
            if (areaWidth < 1 || areaHeight < 1) return MovementCost.Impassable;
            if (mapX < 0 || mapX >= GameMapWidth || mapY < 0 || mapY >= GameMapHeight) return MovementCost.Impassable;

            // Starting values for our movement cost value
            var areaMovementCost = float.MinValue;
            ulong areaInhibition = 0;

            // Collect the tiles we're checking
            for (var x = mapX; x < mapX + areaWidth; x++)
            {
                for (var y = mapY; y < mapY + areaHeight; y++)
                {
                    // Check for collision from game objects or units.
                    FindMapObjectInBounds(out var foundObject, x, y);
                    if (foundObject != null && unit != foundObject)
                    {
                        if (foundObject is IGameUnit)
                        {
                            var otherUnit = foundObject as IGameUnit;
                            if (otherUnit.UnitFaction != unit.UnitFaction) return MovementCost.Impassable;
                        }
                    }

                    // Get the tile group in question
                    var tileIndex = y * GameMapWidth + x;
                    if (tileIndex < 0 || tileIndex >= GameMapTiles.Length || GameMapTiles[tileIndex] == null)
                    {
                        Debug.Log(String.Format("MovementCostForUnit tile index out of bounds or null: {0}. Map tile length: {1}", tileIndex, GameMapTiles.Length));
                        continue;
                    }

                    // Get the movement cost for this tile group
                    var movementCost = TileGroupMovementCostForUnit(unit, GameMapTiles[tileIndex]);

                    // And update our tile area parameters. For an area we use the LARGEST / WORST movement costs across multiple tile groups
                    // If any tile in an area is impassable then the entire area is impassable
                    if (!movementCost.IsPassable) return MovementCost.Impassable;
                    areaMovementCost = Math.Max(areaMovementCost, movementCost.Cost);
                    areaInhibition += movementCost.Inhibition;
                }
            }

            // We have found the worst movement cost for the tile area we have. Return that
            return new MovementCost(areaMovementCost, areaInhibition);
        }


        public MovementCost TileGroupMovementCostForUnit(IGameUnit unit, IGameMapTileGroup gameMapTileGroup)
        {
            // Get the movement cost for this tile group. We want to track if the checked tile is passable and track the cost.
            // Group movemenet cost uses the LOWEST / BEST movement cost and easiest passibility
            var groupCost = float.MaxValue;
            var groupPassable = false;
            ulong groupInhibition = 0;

            foreach (var movementType in unit.MovementTypes)
            {
                var cost = gameMapTileGroup.GetMovementCostForType(movementType);
                if (float.IsNaN(cost)) continue;
                groupPassable = true;
                groupCost = Math.Min(groupCost, cost);
                groupInhibition += gameMapTileGroup.GetInhibitionScoreForUnit(unit);
            }
            // If the tile was found to be impassable then return our impassible object
            if (!groupPassable) return MovementCost.Impassable;
            return new MovementCost(groupCost, groupInhibition);
        }

        #endregion


        #region Movement

        private IEnumerable<IGameMapMovementRoute> _availableRoutes;

        public void ClearMovementOverlay()
        {
            movementOverlay.ClearAllTiles();
            _availableRoutes = null;
        }

        public void ShowUnitMovementRange(IGameUnit unit)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            ClearMovementOverlay();

            _availableRoutes = GetAllRoutesForUnit(unit);

            foreach (var route in _availableRoutes)
            {
                var tilePosition = TranslateMapXYToUnityXY(route.EndX, route.EndY);
                movementOverlay.SetTile(tilePosition, overlayTile);
            }

            stopwatch.Stop();
            Debug.Log(String.Format("Execution time: {0}ms", stopwatch.ElapsedMilliseconds));
        }


        public IEnumerable<IGameMapMovementRoute> GetAllRoutesForUnit(IGameUnit unit)
        {
            // Create our return list
            var routesFound = new Dictionary<int, IGameMapMovementRoute>();

            if (!unit.HasRemainingMovement()) return Enumerable.Empty<IGameMapMovementRoute>();

            // Pull our relevant stats from our unit
            var mapX = unit.MapX;
            var mapY = unit.MapY;
            var availableMovement = unit.RemainingMovement;

            // Create an origin route that we'll extend from here
            var originRoute = new GameMapMovementRoute(mapX, mapY);

            GetRoutesForLocationBreadthSearch(routesFound, originRoute, unit, availableMovement);

            // Now that we found our routes filter out the routes that the user can't end on and return them
            return routesFound.Values.Where(x => x.CanStopHere).ToList();
        }



        public void GetRoutesForLocationBreadthSearch(Dictionary<int, IGameMapMovementRoute> routesFound, IGameMapMovementRoute baseRoute, IGameUnit unit, int availableMovement)
        {

            var searchQueue = new Queue<GetRouteSearchParams>();

            // Queue up our initial directions
            searchQueue.Enqueue(new GetRouteSearchParams(baseRoute, Direction.North));
            searchQueue.Enqueue(new GetRouteSearchParams(baseRoute, Direction.East));
            searchQueue.Enqueue(new GetRouteSearchParams(baseRoute, Direction.South));
            searchQueue.Enqueue(new GetRouteSearchParams(baseRoute, Direction.West));


            while (searchQueue.Any())
            {
                var searchItem = searchQueue.Dequeue();
                ProcessBreadthSearchQueueItem(searchQueue, searchItem, routesFound, unit, availableMovement);
            }

            // Now that we've found all of our routes we need to see which routes have a valid ending point
            foreach (var route in routesFound.Values)
            {
                // Determine if the route can end here
                route.CanStopHere = CanUnitEndMoveHere(unit, route.EndX, route.EndY);
            }
        }

        public void ProcessBreadthSearchQueueItem(Queue<GetRouteSearchParams> searchQueue, GetRouteSearchParams searchParameters, Dictionary<int, IGameMapMovementRoute> routesFound, IGameUnit unit, int availableMovement)
        {
            // still wonderin'
            if (searchParameters.Direction == Direction.None) return;

            // We need to find what tiles we're going to check the movement cost for. Since units cover multiple tiles they have both an area we need to calculate and the edge of the unit we're looking for
            var targetX = searchParameters.BaseRoute.EndX;
            var targetY = searchParameters.BaseRoute.EndY;
            if (searchParameters.Direction == Direction.East) targetX += unit.TileWidth;
            else if (searchParameters.Direction == Direction.West) targetX -= 1;
            else if (searchParameters.Direction == Direction.North) targetY += unit.TileHeight;
            else if (searchParameters.Direction == Direction.South) targetY -= 1;

            // Grab the area that we're going to check
            var areaWidth = DirectionHelper.DirectionVertical(searchParameters.Direction) ? unit.TileWidth : 1;
            var areaHeight = DirectionHelper.DirectionHorizontal(searchParameters.Direction) ? unit.TileHeight : 1;

            // Get the movement cost for this unit to move this direction by one tile
            var movementCost = AreaMovementCostForUnit(unit, targetX, targetY, areaWidth, areaHeight);

            // If the movement cost is IMPASSIBLE or exceeds our available movement then this movement isn't possible
            var totalCost = Mathf.CeilToInt(searchParameters.BaseRoute.RouteCost.Cost + movementCost.Cost); // we do this just as a check here, movement cost can be fractional (TODO: there is a bug here, check half steps logic)
            if (!movementCost.IsPassable || totalCost > availableMovement) return;

            // If it is possible to make this movement then we can create a new route for it!
            var routeHere = searchParameters.BaseRoute.CreateExtendRoute(searchParameters.Direction, movementCost);
            // See if we have an existing route to this ending X,Y
            if (routesFound.ContainsKey(routeHere.EndSortKey))
            {
                var compare = GameMapMovementRouteComparer.Instance.Compare(routeHere, routesFound[routeHere.EndSortKey]);
                if (compare < 0)
                    routesFound[routeHere.EndSortKey] = routeHere;
                else if (compare > 0) return;
                else return; // collision strat: drop latest
            }
            else
            {
                routesFound.Add(routeHere.EndSortKey, routeHere);
            }

            // Find which directions don't overlap the current route
            var openDirections = routeHere.GetOpenDirections();

            // Then add each non-overlapping direction to our queue of searches to perform with this current route
            if (searchParameters.Direction != Direction.North && DirectionFlagsHelper.HasDirectionFlag(openDirections, DirectionFlags.South))
                searchQueue.Enqueue(new GetRouteSearchParams(routeHere, Direction.South));

            if (searchParameters.Direction != Direction.South && DirectionFlagsHelper.HasDirectionFlag(openDirections, DirectionFlags.North))
                searchQueue.Enqueue(new GetRouteSearchParams(routeHere, Direction.North));

            if (searchParameters.Direction != Direction.East && DirectionFlagsHelper.HasDirectionFlag(openDirections, DirectionFlags.West))
                searchQueue.Enqueue(new GetRouteSearchParams(routeHere, Direction.West));

            if (searchParameters.Direction != Direction.West && DirectionFlagsHelper.HasDirectionFlag(openDirections, DirectionFlags.East))
                searchQueue.Enqueue(new GetRouteSearchParams(routeHere, Direction.East));
        }


        public struct GetRouteSearchParams
        {
            public readonly IGameMapMovementRoute BaseRoute;
            public readonly Direction Direction;

            public GetRouteSearchParams(IGameMapMovementRoute baseRoute, Direction direction)
            {
                BaseRoute = baseRoute;
                Direction = direction;
            }
        }


        public bool CanUnitEndMoveHere(IGameUnit unit, int mapX, int mapY)
        {
            // The unit can't end their move on impassible tiles, but they can't have a route that ends on them anyways because they're impassible!
            // Ergo we only need to check on a tile that we can move over but can't end their turn on, which would be tiles occupied by other units.
            // Not sure exactly the final rules of this mechanic, for now we'll assume all friendly units are passable.

            for (var x = mapX; x < mapX + unit.TileWidth; x++)
            {
                for (var y = mapY; y < mapY + unit.TileHeight; y++)
                {
                    if (!FindMapObjectInBounds(out var foundObject, x, y)) continue;
                    if (foundObject == unit) continue;
                    return false;

                }
            }
            return true;
        }


        #endregion


        public List<IGameMapObject> MapObjects { get; } = new List<IGameMapObject>();
        public List<IGameUnit> MapUnits { get; } = new List<IGameUnit>();


        #region Load Events

        private bool _isLoaded = false;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set
            {
                if (value == _isLoaded) return;
                _isLoaded = value;
                if (_isLoaded) OnLoaded();
            }
        }


        public event GameMapLoadedHandler Loaded;
        protected void OnLoaded()
        {
            Loaded?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        #region Unity Start & Update

        // Start is called before the first frame update
        public void Start()
        {
            GetDepenencies();
            _mapcamera = Camera.main;

            //if (BaseTileMap == null) throw new NullReferenceException(nameof(BaseTileMap));
            //BaseTileMap.ClearAllTiles();

            // Populate our internal data structures to track what's going on in the map
            RefreshGameMapTileArray();
            RefreshMapUnits();
            RefreshMapCursor();

            IsLoaded = true;
        }


        public void Update()
        {
            var mouseInView = IsMouseInView();

            if(IsCursorEnabled && mouseInView)
            {
                UpdateMapCursor();
            }

            if(mouseInView)
            {
                UpdateMousePoint();

                
                UpdateUserInput();
            }
        }

        #endregion


        #region Plyaer Input, Unit Selection and Commands

        private IGameUnit _selectedUnit;

        private MapPoint _lastMousePosition;
        private MapPoint _currentMousePosition;


        /// <summary>
        /// Check if the the primary input for the player is in view of the game window.
        /// </summary>
        /// <returns>Returns true if the user input is over the game window, false otherwise.</returns>
        public bool IsMouseInView()
        {
            var viewportPoint = _mapcamera.ScreenToViewportPoint(Input.mousePosition);
            return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
        }



        public void UpdateMousePoint()
        {
            // Remember our last mouse position
            _lastMousePosition = _currentMousePosition;

            _currentMousePosition = GetPlayerInputMapPoint();
        }


        


        /// <summary>
        /// Get the map point the user is focusing on based on the player's input, such as the location of the mouse cursor
        /// </summary>
        /// <returns>A 1x1 map point for where the player's input is</returns>
        public MapPoint GetPlayerInputMapPoint()
        {
            // Get the position of the mouse in world coordinates
            var point = _mapcamera.ScreenToWorldPoint(Input.mousePosition);
            // And convert it to tile coordinates
            var tilePosition = objectTileMap.WorldToCell(point);

            // Convert our grid position into map data X,Y positions which is how we're storing positional information
            TranslateUnityXYToMapXY(tilePosition, out var mapX, out var mapY);

            return new MapPoint(mapX, mapY, 1, 1);
        }


        public void UpdateUserInput()
        {
            if (!_gameSceneBattle.IsPlayerEnabledPhase()) return;

            // Back button hit
            if(Input.GetMouseButtonDown(1))
            {
                OnBackButton();
                return;
            }

            if (!Input.GetMouseButtonDown(0)) return;




            // If the player selected a game object
            if (FindMapObjectInBounds(out var foundObject, _currentMousePosition.X, _currentMousePosition.Y))
            {
                var foundUnit = foundObject as IGameUnit;
                OnSelectedUnit(foundUnit);
                return;
            }

            // If the player selected a game route
            if(_availableRoutes != null)
            {
                var selectedRoute = _availableRoutes.FirstOrDefault(x => x.EndX == _currentMousePosition.X && x.EndY == _currentMousePosition.Y);
                if(selectedRoute != null)
                {
                    OnSelectedRoute(selectedRoute);
                    return;
                }
            }

            // Fallthrough: If the player selected empty space
            OnSelectedEmptySpace();

        }


        public event EventHandler BackButton;

        public void OnBackButton()
        {
            BackButton?.Invoke(this, EventArgs.Empty);
        }


        public void UpdateUserInput2()
        {
            if (!_gameSceneBattle.IsPlayerEnabledPhase()) return;

            if (!Input.GetMouseButtonDown(0)) return;


            // If we selected a game object
            if (FindMapObjectInBounds(out var foundObject, _currentMousePosition.X, _currentMousePosition.Y))
            {
                var foundUnit = foundObject as IGameUnit;
                if (_selectedUnit == foundUnit) return;
                if (_selectedUnit != null && _selectedUnit != foundUnit) ClearMovementOverlay();

                _selectedUnit = foundUnit;
                ShowUnitMovementRange(_selectedUnit);
                //selectedUnitAbilitiesWindow.UpdateSelectedUnit(_selectedUnit); //TODO
                return;
            }

            // The user didn't select a unit, see if they clicked on a visible route and move them if they can
            if(_selectedUnit != null && _availableRoutes != null && _selectedUnit.CanMoveAction() && _gameSceneBattle.CanUnitAct(_selectedUnit))
            {
                var selectedRoute = _availableRoutes.FirstOrDefault(x => x.EndX == _currentMousePosition.X && x.EndY == _currentMousePosition.Y);
                if(selectedRoute != null)
                {
                    // They clicked on a route for a unit so move them!
                    StartCoroutine(MoveUnitCoroutine(_selectedUnit, selectedRoute));
                    return;
                }
            }
            
            // Otherwise they selected empty space
            SelectEmptySpace();

        }

        public void SelectEmptySpace()
        {
            // If we have a unit selected and they haven't moved yet then unselect the unit
            if(_selectedUnit != null)
            {
                UnselectUnit();
            }

            // Otherwise there's nothing to do yet
        }

        public void UnselectUnit()
        {
            _selectedUnit = null;
            _availableRoutes = null;
            ClearMovementOverlay();
            //selectedUnitAbilitiesWindow.ClearSelectedUnit(); TODO
        }


        public event SelectedUnitHandler PlayerSelectedUnit;

        public void OnSelectedUnit(IGameUnit unit)
        {
            PlayerSelectedUnit?.Invoke(this, new UnitSelectedEventArgs(unit));
        }

        public void OnSelectedEmptySpace()
        {
            PlayerSelectedUnit?.Invoke(this, UnitSelectedEventArgs.Empty);
        }


        public event SelectedRouteHandler PlayerSelectedRoute;

        public void OnSelectedRoute(IGameMapMovementRoute route)
        {
            PlayerSelectedRoute?.Invoke(this, new RouteSelectedEventArgs(route));
        }

        public void OnSelectedNoRoute()
        {
            PlayerSelectedRoute?.Invoke(this, RouteSelectedEventArgs.Empty);
        }

        public void MoveUnit(IGameUnit unit, IGameMapMovementRoute route)
        {
            StartCoroutine(MoveUnitCoroutine(unit, route));
        }

        //public async Task MoveUnitAsync(IGameUnit unit, IGameMapMovementRoute route)
        //{
        //    await MoveUnitCoroutine(unit, route);
        //}


        public IEnumerator MoveUnitCoroutine(IGameUnit unit, IGameMapMovementRoute route)
        {
            // Unselect the unit we're moving
            UnselectUnit();

            var hasWalkingDamage = unit.GetAttribute(UnitAttribute.WalkingDamage) > 0;

            foreach (var routeStep in route.Route.Where(x => x != Direction.None))
            {
                var startPosition = unit.WorldPosition;
                var directionVector = DirectionHelper.GetDirectionVector(routeStep);
                var targetPosition = unit.WorldPosition + directionVector;

                var duration = 0.15f;
                var stepTime = 0f;

                while (stepTime < duration)
                {
                    stepTime += Time.deltaTime;
                    unit.WorldPosition = Vector3.Lerp(startPosition, targetPosition, stepTime / duration);
                    yield return null;
                }

                // Animation complete, set the unit position to be the end location
                unit.WorldPosition = targetPosition;
                var newX = unit.MapX;
                var newY = unit.MapY;
                DirectionHelper.ApplyDirection(routeStep, ref newX, ref newY);
                unit.MapX = newX;
                unit.MapY = newY;

                if(hasWalkingDamage)
                {
                    ApplyWalkingDamage(unit, newX, newY, unit.TileWidth, unit.TileHeight);
                }
            }

            // Final snap the unit to their end map & unity location
            unit.MapX = route.EndX;
            unit.MapY = route.EndY;
            unit.WorldPosition = TranslateMapXYToUnityXY(unit.MapX, unit.MapY);
            // And tell them to consume movement for this route
            unit.AddRouteCost(route);
            // Trigger the MoveCompleted event now that we're done animating the move
            OnUnitMoveCompleted();
        }

        public event EventHandler UnitMoveCompleted;
        public void OnUnitMoveCompleted()
        {
            UnitMoveCompleted?.Invoke(this, EventArgs.Empty);
        }

        public void ApplyWalkingDamage(IGameUnit unit, int mapX, int mapY, int areaWidth, int areaHeight)
        {
            var damage = unit.GetAttribute(UnitAttribute.WalkingDamage);
            if (damage == 0) return;

            // Collect the tiles we're checking
            for (var x = mapX; x < mapX + areaWidth; x++)
            {
                for (var y = mapY; y < mapY + areaHeight; y++)
                {
                    // Get the tile group in question
                    var tileIndex = y * GameMapWidth + x;
                    if (tileIndex < 0 || tileIndex >= GameMapTiles.Length || GameMapTiles[tileIndex] == null)
                    {
                        Debug.Log(String.Format("MovementCostForUnit tile index out of bounds or null: {0}. Map tile length: {1}", tileIndex, GameMapTiles.Length));
                        continue;
                    }

                    var tileGroup = GameMapTiles[tileIndex];

                    foreach (var tile in tileGroup.Tiles.Where(z => z != null))
                    {
                        tile.InflictDamage(damage, unit);
                    }
                }
            }
        }

        #endregion


        #region Map Cursor Code

        private Vector3Int _lastCursorCellPosition;
        private Vector3 _cursorOffsetVector = new Vector3(0f, 1f);


        private bool _isCursorEnabled;
        public bool IsCursorEnabled
        {
            get { return _isCursorEnabled; }
            set
            {
                if (value == _isCursorEnabled) return;
                _isCursorEnabled = value;
                RefreshMapCursor();
            }
        }


        private IGameMapObject _cursorHoverObject;
        public IGameMapObject CursorHoverObject
        {
            get { return _cursorHoverObject; }
            private set
            {
                if (value == _cursorHoverObject) return;
                _cursorHoverObject = value;
            }
        }

        

        private IGameUnit _cursorHoverUnit;
        public IGameUnit CursorHoverUnit
        {
            get { return _cursorHoverUnit; }
            private set
            {
                if (value == _cursorHoverUnit) return;
                _cursorHoverUnit = value;
                OnHoverUnitChanged(_cursorHoverUnit);

            }
        }

        public event SelectedUnitHandler HoverUnitChanged;

        private void OnHoverUnitChanged(IGameUnit unit)
        {
            HoverUnitChanged?.Invoke(this, new UnitSelectedEventArgs(unit));
        }

        public void RefreshMapCursor()
        {
            gameMapCursor.CursorEnabled = IsCursorEnabled;

            // WHen the cursor is turned off then remove any hovered units too
            if(!IsCursorEnabled)
            {
                CursorHoverObject = null;
                CursorHoverUnit = null;
                _lastCursorCellPosition = Vector3Int.one; // v3int.one is an impossible value so it's good as an always-invalid value for comparison when the cursor is enabled later on
            }
        }



        public void UpdateMapCursor()
        {
            // Get the position of the mouse in world coordinates
            
            var point = _mapcamera.ScreenToWorldPoint(Input.mousePosition);
            // And convert it to tile coordinates
            var tilePosition = objectTileMap.WorldToCell(point);

            // If the tile coordinates haven't changed since our last update then we shouldn't do any work since it should have the same results
            if (_lastCursorCellPosition == tilePosition) return;

            // Convert our grid position into map data X,Y positions which is how we're storing positional information
            TranslateUnityXYToMapXY(tilePosition, out var mapX, out var mapY);

            // Find an object to highlight. If there is a unit the cursor should match it's position and tile width/height via scaling
            if (FindMapObjectInBounds(out var foundObject, mapX, mapY))
            {
                var foundUnit = foundObject as GameUnit;
                SetCursorPositionOnUnit(foundUnit);

                CursorHoverUnit = foundUnit;
            }
            else
            {
                // If no unit was found move the cursor to the cell that's highlighted
                var cursorPostion = objectTileMap.CellToWorld(tilePosition);
                SetCursorPosition(cursorPostion);

                CursorHoverUnit = null;
            }

            // Remember our cell cordinates to try and avoid doing extra work next update
            _lastCursorCellPosition = tilePosition;
        }

        public void SetCursorPositionOnUnit(GameUnit unit)
        {
            SetCursorPosition(new Vector3(unit.transform.position.x, unit.transform.position.y + unit.TileHeight - 1, gameMapCursor.transform.position.z), new Vector3(unit.TileWidth, unit.TileHeight, 1f));
        }

        public void SetCursorPosition(Vector3 newPosition)
        {
            SetCursorPosition(newPosition, Vector3.one);
        }

        public void SetCursorPosition(Vector3 newPosition, Vector3 scale)
        {
            var adjustedVector = newPosition + _cursorOffsetVector;
            gameMapCursor.transform.position = adjustedVector;

            gameMapCursor.transform.localScale = scale;
        }

        #endregion


        public bool FindMapObjectInBounds(out IGameMapObject foundObject, int mapX, int mapY)
        {
            foreach (var mapObject in MapObjects)
            {
                // Simple check: Object is not visible or hidden then skip checking this item
                if (mapObject.TileWidth < 1 || mapObject.TileHeight < 1) continue;

                // Simple check: The unit is 1x1
                if (mapObject.TileWidth == 1 && mapObject.TileHeight == 1 && mapObject.MapX == mapX && mapObject.MapY == mapY)
                {
                    foundObject = mapObject;
                    return true;
                }


                // The map object has dimensions, we need to see if our target X/Y falls within its bounds
                var leftBounds = mapObject.MapX;
                var rightBounds = leftBounds + (mapObject.TileWidth - 1);
                var topBounds = mapObject.MapY + (mapObject.TileHeight - 1);
                var bottomBounds = mapObject.MapY;

                if (mapX >= leftBounds && mapX <= rightBounds && mapY <= topBounds && mapY >= bottomBounds)
                {
                    foundObject = mapObject;
                    return true;
                }
            }

            // Fallthrough: No units were found
            foundObject = null;
            return false;
        }


        public void RefreshMapUnits()
        {

            var mapUnits = objectTileMap.GetComponentsInChildren<GameUnit>();
            if (mapUnits.Any())
            {
                foreach (var unit in mapUnits)
                {
                    // Add the unit to our list of units and objects on this map
                    if (!MapUnits.Contains(unit))
                        MapUnits.Add(unit);
                    if (!MapObjects.Contains(unit))
                        MapObjects.Add(unit);
                    // And make sure it's Map X/Y coordinates are corred in our grid
                    var unitPosition = objectTileMap.WorldToCell(unit.transform.position);
                    var gridPosition = objectTileMap.CellToWorld(unitPosition);
                    unit.transform.position = gridPosition;
                    TranslateUnityXYToMapXY(unitPosition, out var unitMapX, out var unitMapY);
                    unit.MapX = unitMapX;
                    unit.MapY = unitMapY;

                }
            }
        }

    }

}
