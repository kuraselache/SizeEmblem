using SizeEmblem.Assets.Scripts.GameMap;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.Events.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Scripts.Interfaces.GameMap
{
    public interface IGameMap
    {
        bool IsLoaded { get; }
        event GameMapLoadedHandler Loaded;

        int GameMapHeight { get; }
        int GameMapWidth { get; }

        int GameMapOriginX { get; }
        int GameMapOriginY { get; }

        int MapLayers { get; }
        IGameMapTileGroup[] GameMapTiles { get; }

        List<IGameMapObject> MapObjects { get; }
        List<IGameUnit> MapUnits { get; }



        MovementCost AreaMovementCostForUnit(IGameUnit unit, int mapX, int mapY, int areaWidth = 1, int areaHeight = 1);
        bool CanUnitEndMoveHere(IGameUnit unit, int mapX, int mapY);
        
        void FindGameSceneBattle();

        
        IEnumerable<IGameMapMovementRoute> GetAllRoutesForUnit(IGameUnit unit);
        void GetDepenencies();
        void GetRoutesForLocationBreadthSearch(Dictionary<int, IGameMapMovementRoute> routesFound, IGameMapMovementRoute baseRoute, IGameUnit unit, int availableMovement);
        void LoadMapLayer(IGameMapTileGroup[] gameMapTiles, int gmWidth, int gmHeight, int mapLayer);
        
        //void ProcessBreadthSearchQueueItem(Queue<GameMap.GetRouteSearchParams> searchQueue, GameMap.GetRouteSearchParams searchParameters, Dictionary<int, IGameMapMovementRoute> routesFound, IGameUnit unit, int availableMovement);
        void RefreshGameMapTileArray();
        void RefreshMapUnits();
        void UpdateUserInput();
        void SetCursorPosition(Vector3 newPosition);
        
        void Start();
        MovementCost TileGroupMovementCostForUnit(IGameUnit unit, IGameMapTileGroup gameMapTileGroup);
        Vector3Int TranslateMapXYToUnityXY(int mapX, int mapY);
        void TranslateUnityXYToMapXY(int unityX, int unityY, out int mapX, out int mapY);
        void TranslateUnityXYToMapXY(Vector3Int unityPosition, out int mapX, out int mapY);
        void Update();


        // Finding Map Objects
        bool GetUnitAtCursor(out IGameUnit foundUnit);
        bool FindMapObjectInBounds(out IGameMapObject foundObject, int mapX, int mapY);

        // Player Input
        event SelectedUnitHandler PlayerSelectedUnit;
        event SelectedRouteHandler PlayerSelectedRoute;

        // Unit Movement Related
        void ShowUnitMovementRange(IGameUnit unit);
        void ClearMovementOverlay();


        void SetUnitLocation(IGameUnit unit, int x, int y);
        void MoveUnit(IGameUnit unit, IGameMapMovementRoute route);
        event EventHandler<UnitMoveCompletedEventArgs> UnitMoveCompleted;

        // Cursor Methods
        bool IsCursorEnabled { get; set; }
        void UpdateMapCursor();

        IGameUnit CursorHoverUnit { get; }
        IGameMapObject CursorHoverObject { get; }

        event SelectedUnitHandler HoverUnitChanged;

        // Ability Range
        void SetAbilityRange(GameMapAbilityRange abilityRange);
        void DrawAbilityRange(GameMapAbilityRange abilityRange);
        void ClearAbilityRange();
    }
}
