using Assets.Scripts.Interfaces.GameMap;
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
using SizeEmblem.Scripts.UI;
using SizeEmblem.Scripts.GameUnits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SizeEmblem.Scripts.GameMap
{
    public class GameMap : MonoBehaviour, IGameMap
    {
        #region Unity Dependencies

        private Camera _mapcamera;


        public UISelectedUnitSummary uiSelectedUnitSummary;

        public Tilemap baseTileMap;
        public Tilemap[] baseTileMaps;

        public Tilemap objectTileMap;

        public Tilemap movementOverlay;
        public TileBase overlayTile;

        public GameMapCursor gameMapCursor;

        #endregion

        #region Code Dependencies

        private IInputManager _inputManager;
        private IGameMapTileGroupFactory _gameMapTileGroupFactory;

        public void GetDepenencies()
        {
            // Add DI eventually here
            _inputManager = new UnityInputManager();
            _gameMapTileGroupFactory = new GameMapTileGroupFactory();
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
            if(GameMapTiles == null || GameMapTiles.Length != GameMapWidth * GameMapHeight)
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
            
            foreach(var mapTile in mapTiles)
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
            uint groupInhibition = 0;

            foreach (var movementType in unit.MovementTypes)
            {
                var cost = gameMapTileGroup.GetMovementCostForType(movementType);
                if (float.IsNaN(cost)) continue;
                groupPassable = true;
                groupCost = Math.Min(groupCost, cost);
                groupInhibition += gameMapTileGroup.GetInhibitionScoreForUnit(unit);
            }
            // If the tile was found to be impassable then return NaN for movement cost
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

            foreach(var route in _availableRoutes)
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

            if (!unit.CanMove()) return Enumerable.Empty<IGameMapMovementRoute>();

            // Pull our relevant stats from our unit
            var mapX = unit.MapX;
            var mapY = unit.MapY;
            var availableMovement = unit.RemainingMovement;

            // Create an origin route that we'll extend from here
            var originRoute = new GameMapMovementRoute(mapX, mapY);

            // Get the routes from all directions from our origin route
            //GetRoutesForLocationDepthSearch(routesFound, originRoute, Direction.North, unit, availableMovement);
            //GetRoutesForLocationDepthSearch(routesFound, originRoute, Direction.East,  unit, availableMovement);
            //GetRoutesForLocationDepthSearch(routesFound, originRoute, Direction.South, unit, availableMovement);
            //GetRoutesForLocationDepthSearch(routesFound, originRoute, Direction.West,  unit, availableMovement);

            GetRoutesForLocationBreadthSearch(routesFound, originRoute, unit, availableMovement);

            // Now that we found our routes we need to narrow them down so there's only one route for each End X,Y. We need to group our end X,Ys together
            return routesFound.Values.ToList();
        }




        public void GetRoutesForLocationDepthSearch(Dictionary<int, IGameMapMovementRoute> routesFound, IGameMapMovementRoute baseRoute, Direction direction, IGameUnit unit, int availableMovement)
        {
            // sometimes I wonder what I'm thinking
            if (direction == Direction.None) return;

            // We need to find what tiles we're going to check the movement cost for. Since units cover multiple tiles they have both an area we need to calculate and the edge of the unit we're looking for
            var targetX = baseRoute.EndX;
            var targetY = baseRoute.EndY;
            if      (direction == Direction.East)  targetX += unit.TileWidth;
            else if (direction == Direction.West)  targetX -= unit.TileWidth;
            else if (direction == Direction.North) targetY += unit.TileHeight;
            else if (direction == Direction.South) targetY -= unit.TileHeight;

            // Grab the area that we're going to check
            var areaWidth  = DirectionHelper.DirectionVertical(direction)   ? unit.TileWidth  : 1;
            var areaHeight = DirectionHelper.DirectionHorizontal(direction) ? unit.TileHeight : 1;

            // Get the movement cost for this unit to move this direction by one tile
            var movementCost = AreaMovementCostForUnit(unit, targetX, targetY, areaWidth, areaHeight);

            // If the movement cost is IMPASSIBLE or exceeds our available movement then this movement isn't possible
            var totalCost = Mathf.CeilToInt(baseRoute.RouteCost.Cost + movementCost.Cost); // we do this just as a check here, movement cost can be fractional (TODO: there is a bug here, check half steps logic)
            if (!movementCost.IsPassable || totalCost > availableMovement) return;

            // If it is possible to make this movement then we can create a new route for it!
            var routeHere = baseRoute.CreateExtendRoute(direction, movementCost);
            // See if we have an existing route to this ending X,Y
            if(routesFound.ContainsKey(routeHere.EndSortKey))
            {
                var compare = GameMapMovementRouteComparer.Instance.Compare(routeHere, routesFound[routeHere.EndSortKey]);
                if (compare < 0)
                    routesFound[routeHere.EndSortKey] = routeHere;
                else if (compare > 0) return;
            }
            else
            {
                routesFound.Add(routeHere.EndSortKey, routeHere);
            }

            // Now see if we can go any other directions from here that won't overlap our base route
            if(direction != Direction.North && routeHere.CheckDirectionForOverlap(Direction.South))
            {
                GetRoutesForLocationDepthSearch(routesFound, routeHere, Direction.South, unit, availableMovement);
            }

            if (direction != Direction.South && routeHere.CheckDirectionForOverlap(Direction.North))
            {
                GetRoutesForLocationDepthSearch(routesFound, routeHere, Direction.North, unit, availableMovement);
            }

            if (direction != Direction.East && routeHere.CheckDirectionForOverlap(Direction.West))
            {
                GetRoutesForLocationDepthSearch(routesFound, routeHere, Direction.West, unit, availableMovement);
            }

            if (direction != Direction.West && routeHere.CheckDirectionForOverlap(Direction.East))
            {
                GetRoutesForLocationDepthSearch(routesFound, routeHere, Direction.East, unit, availableMovement);
            }

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
        }

        public void ProcessBreadthSearchQueueItem(Queue<GetRouteSearchParams> searchQueue, GetRouteSearchParams searchParameters, Dictionary<int, IGameMapMovementRoute> routesFound, IGameUnit unit, int availableMovement)
        {
            // still wonderin'
            if (searchParameters.Direction == Direction.None) return;

            // We need to find what tiles we're going to check the movement cost for. Since units cover multiple tiles they have both an area we need to calculate and the edge of the unit we're looking for
            var targetX = searchParameters.BaseRoute.EndX;
            var targetY = searchParameters.BaseRoute.EndY;
            if      (searchParameters.Direction == Direction.East)  targetX += unit.TileWidth;
            else if (searchParameters.Direction == Direction.West)  targetX -= unit.TileWidth;
            else if (searchParameters.Direction == Direction.North) targetY += unit.TileHeight;
            else if (searchParameters.Direction == Direction.South) targetY -= unit.TileHeight;

            // Grab the area that we're going to check
            var areaWidth  = DirectionHelper.DirectionVertical(searchParameters.Direction)   ? unit.TileWidth  : 1;
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

            // Queue up more work
            if (searchParameters.Direction != Direction.North && routeHere.CheckDirectionForOverlap(Direction.South))
            {
                searchQueue.Enqueue(new GetRouteSearchParams(routeHere, Direction.South));
            }

            if (searchParameters.Direction != Direction.South && routeHere.CheckDirectionForOverlap(Direction.North))
            {
                searchQueue.Enqueue(new GetRouteSearchParams(routeHere, Direction.North));
            }

            if (searchParameters.Direction != Direction.East && routeHere.CheckDirectionForOverlap(Direction.West))
            {
                searchQueue.Enqueue(new GetRouteSearchParams(routeHere, Direction.West));
            }

            if (searchParameters.Direction != Direction.West && routeHere.CheckDirectionForOverlap(Direction.East))
            {
                searchQueue.Enqueue(new GetRouteSearchParams(routeHere, Direction.East));
            }
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

        #endregion


        public List<IGameMapObject> MapObjects { get; } = new List<IGameMapObject>();
        public List<GameUnit> MapUnits { get; } = new List<GameUnit>();


        

        // Start is called before the first frame update
        public void Start()
        {
            GetDepenencies();
            _mapcamera = Camera.main;

            //if (BaseTileMap == null) throw new NullReferenceException(nameof(BaseTileMap));
            //BaseTileMap.ClearAllTiles();

            // Populate our internal data structures to track what's going on in the map
            RefreshGameMapTileArray();
            UpdateMapUnits();
        }


        public void Update()
        {
            gameMapCursor.CursorEnabled = true;

            UpdateMapCursor();
            SelectUnit();
        }


        #region Select Unit

        private IGameUnit _selectedUnit;

        public void SelectUnit()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            // Get the position of the mouse in world coordinates
            var point = _mapcamera.ScreenToWorldPoint(Input.mousePosition);
            // And convert it to tile coordinates
            var tilePosition = objectTileMap.WorldToCell(point);

            // Convert our grid position into map data X,Y positions which is how we're storing positional information
            TranslateUnityXYToMapXY(tilePosition, out var mapX, out var mapY);

            if (FindMapUnitInBounds(out var foundObject, mapX, mapY))
            {
                var foundUnit = foundObject as IGameUnit;
                if (_selectedUnit == foundUnit) return;
                if (_selectedUnit != null && _selectedUnit != foundUnit) ClearMovementOverlay();

                _selectedUnit = foundUnit;
                ShowUnitMovementRange(_selectedUnit);
            }
            else
            {
                if (_selectedUnit == null) return;

                var route = _availableRoutes.FirstOrDefault(x => x.EndX == mapX && x.EndY == mapY);
                if (route == null) return;

                Debug.Log("Found a route to the selected square!");
                var unit = _selectedUnit;
                _selectedUnit = null;
                StartCoroutine(MoveUnitCoroutine(unit, route));
            }
        }

        public IEnumerator MoveUnitCoroutine(IGameUnit unit, IGameMapMovementRoute route)
        {
            ClearMovementOverlay();
            _availableRoutes = null;

            foreach (var routeStep in route.Route.Where(x => x != Direction.None))
            {
                var directionVector = DirectionHelper.GetDirectionVector(routeStep);
                var targetPosition = unit.WorldPosition + directionVector;

                var duration = 0.15f;
                var stepTime = 1f * duration;

                while(stepTime > 0)
                {
                    var delta = Time.deltaTime;
                    stepTime -= delta;
                    unit.WorldPosition += (directionVector * (delta / duration));
                    yield return null;
                }
                // Animation complete, set the unit position to be the end location
                unit.WorldPosition = targetPosition;
                var newX = unit.MapX;
                var newY = unit.MapY;
                DirectionHelper.ApplyDirection(routeStep, ref newX, ref newY);
                unit.MapX = newX;
                unit.MapY = newY;

                var areaWidth = 1;
                var areaHeight = 1;
                if (unit.TileHeight > 1 && DirectionHelper.DirectionHorizontal(routeStep)) areaHeight = unit.TileHeight;
                if (unit.TileWidth > 1 && DirectionHelper.DirectionVertical(routeStep)) areaWidth = unit.TileWidth;

                ApplyWalkingDamage(unit, newX, newY, areaWidth, areaHeight);
            }

            unit.MapX = route.EndX;
            unit.MapY = route.EndY;
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

                    foreach(var tile in tileGroup.Tiles.Where(z => z != null))
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


        public void UpdateMapCursor()
        {
            // Get the position of the mouse in world coordinates
            var point = _mapcamera.ScreenToWorldPoint(Input.mousePosition);
            // And convert it to tile coordinates
            var tilePosition = objectTileMap.WorldToCell(point);

            // If the tile coordinates haven't changed since our last update then we shouldn't do any work since it should have the same results
            if(_lastCursorCellPosition == tilePosition) return;

            // Convert our grid position into map data X,Y positions which is how we're storing positional information
            TranslateUnityXYToMapXY(tilePosition, out var mapX, out var mapY);

            // Is this necessary? Should be the same value but as floats... which we cast back to ints in a second
            //var cursorPostion = objectTileMap.CellToWorld(tilePosition);

            // Find an object to highlight. If there is a unit the cursor should match it's position and tile width/height via scaling
            if (FindMapUnitInBounds(out var foundObject, mapX, mapY))
            {
                var foundUnit = foundObject as GameUnit;
                SetCursorPosition(new Vector3(foundUnit.transform.position.x, foundUnit.transform.position.y + foundUnit.TileHeight - 1, gameMapCursor.transform.position.z));
                gameMapCursor.transform.localScale = new Vector3(foundUnit.TileWidth, foundUnit.TileHeight, 1f);

                uiSelectedUnitSummary.SelectedUnit = foundUnit;
            }
            else
            {
                // If no unit was found move the cursor to the cell that's highlighted
                var cursorPostion = objectTileMap.CellToWorld(tilePosition);
                SetCursorPosition(cursorPostion);
                gameMapCursor.transform.localScale = Vector3.one;
                uiSelectedUnitSummary.SelectedUnit = null;
            }

            // Remember our cell cordinates to try and avoid doing extra work next update
            _lastCursorCellPosition = tilePosition;
        }


        
        public void SetCursorPosition(Vector3 newPosition)
        {
            var adjustedVector = newPosition + _cursorOffsetVector;
            gameMapCursor.transform.position = adjustedVector;
        }

        #endregion


        public bool FindMapUnitInBounds(out IGameMapObject foundObject, int mapX, int mapY)
        {
            foreach(var mapObject in MapObjects)
            {
                // Simple check: Object is not visible or hidden then skip checking this item
                if (mapObject.TileWidth < 1 || mapObject.TileHeight < 1) continue;

                // Simple check: The unit is 1x1
                if(mapObject.TileWidth == 1 && mapObject.TileHeight == 1 && mapObject.MapX == mapX && mapObject.MapY == mapY)
                {
                    foundObject = mapObject;
                    return true;
                }


                // The map object has dimensions, we need to see if our target X/Y falls within its bounds
                var leftBounds   = mapObject.MapX;
                var rightBounds  = leftBounds + (mapObject.TileWidth - 1);
                var topBounds    = mapObject.MapY + (mapObject.TileHeight - 1);
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


        public void UpdateMapUnits()
        {

            var mapUnits = objectTileMap.GetComponentsInChildren<GameUnit>();
            if(mapUnits.Any())
            {
                foreach(var unit in mapUnits)
                {
                    // Add the unit to our list of units and objects on this map
                    if(!MapUnits.Contains(unit))
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
