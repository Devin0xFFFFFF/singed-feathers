using System.Collections.Generic;
using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public interface IMapController {
        int Width { get; }
        int Height { get; }
        bool GenerateMap(string serializedMap);
        void ApplyHeat(int x, int y);
        TileType GetTileType(int x, int y);
        ITileController GetTileController(int x, int y);
        IList<IPigeonController> GetPigeonControllers();
        ITurnResolver GetTurnResolver();
        ITurnController GetTurnController();
        void SetTurnResolver(ITurnResolver turnResolver);
        void EndTurn();
        int GetTurnsLeft();
        void UndoAllActions();
        void Fire();
        void Water();
        void Cancel();
    }
}