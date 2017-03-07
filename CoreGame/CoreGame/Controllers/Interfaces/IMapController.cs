using System.Collections.Generic;
using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public interface IMapController {
        int Width { get; }
        int Height { get; }
        void GenerateMap();
        void ApplyHeat(int x, int y);
        TileType GetTileType(int x, int y);
        ITileController GetTileController(int x, int y);
        IDictionary<NewStatus, IList<Position>> SpreadFires();
        IList<IPigeonController> GetPigeonControllers();
        ITurnResolver GetTurnResolver();
        ITurnController GetTurnController();
        void MovePigeons();
        Position GetInitialFirePosition();
        void EndTurn();
        int GetTurnsLeft();
        void UndoAllActions();
        void Fire();
        void Water();
        void Cancel();
    }
}