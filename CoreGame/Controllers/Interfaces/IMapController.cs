using System.Collections.Generic;
using CoreGame.Models;

namespace CoreGame.Controllers.Interfaces {
    public interface IMapController {
        int Width { get; }
        int Height { get; }
        bool GenerateMap(string serializedMap);
        void SetPlayerSideSelection(PlayerSideSelection playerSideSelection);
        PlayerSideSelection GetPlayerSideSelection();
        string GetGameOverPlayerStatus();
        bool IsMapBurntOut();
        bool AreAllPigeonsDead();
        void ApplyHeat(int x, int y);
        void ApplyDelta(IList<Delta> deltaList);
        TileType GetTileType(int x, int y);
        ITileController GetTileController(int x, int y);
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