using System.Collections.Generic;
using CoreGame.Models;

namespace CoreGame.Controllers.Interfaces {
    public interface IMapController {
        int Width { get; }
        int Height { get; }
        bool GenerateMap(string serializedMap);
        bool GenerateDefaultMap();
        string SerializeMap();
        void SetPlayerSideSelection(PlayerSideSelection playerSideSelection);
        PlayerSideSelection GetPlayerSideSelection();
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
        bool UpdateTileController(TileType type, int x, int y);
        bool AddInitialPigeonPosition(Position position);
        bool RemoveInitialPigeonPosition(Position position);
        bool AddInitialFirePosition(Position position);
        bool RemoveInitialFirePosition(Position position);
        bool UpdateNumberOfTurns(int numTurns);
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