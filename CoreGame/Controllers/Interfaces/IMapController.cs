using System.Collections.Generic;
using CoreGame.Models;

namespace CoreGame.Controllers.Interfaces {
    public interface IMapController {
        int Width { get; }
        int Height { get; }
        bool GenerateMap(string serializedMap);
        bool GenerateDefaultMap();
        string SerializeMap(int width, int height, IList<Position> pigeonPositions, IList<Position> firePositions, TileType[,] tileMap, int numTurns);
        void SetPlayerSideSelection(PlayerSideSelection playerSideSelection);
        PlayerSideSelection GetPlayerSideSelection();
        string GetPlayerName();
        bool IsGameOver();
        string GetGameOverPlayerStatus();
        bool IsMapBurntOut();
        bool AreAllPigeonsDead();
        int GetLivePigeonCount();
        void ApplyHeat(int x, int y);
        void ApplyDelta(IList<Delta> deltaList);
        void ReduceHeat(int x, int y);
        TileType GetTileType(int x, int y);
        ITileController GetTileController(int x, int y);
        void UpdateTileController(TileType type, int x, int y);
        PigeonController AddPigeonToMap(Position position);
        IList<IPigeonController> GetPigeonControllers();
        ITurnResolver GetTurnResolver();
        ITurnController GetTurnController();
        void SetTurnResolver(ITurnResolver turnResolver);
        void EndTurn();
        int GetTurnsLeft();
        void UndoAction();
        void Fire();
        void Water();
        bool IsTurnResolved();
        bool ShouldPoll();
        bool ValidateDelta(Delta delta);
        void Poll();
    }
}