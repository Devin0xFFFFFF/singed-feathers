using Assets.Scripts.Models;
using Assets.Scripts.Models.Commands;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Controllers
{
    public class TurnController : ITurnController {
        private MoveTypes _moveType;
        private ICommand _command;
        private int _intensity = 100;
        private int _maxMoves;
        private int _turnsLeft;
        private IDictionary<ITileController, ICommand> _moves;

        public TurnController(int turnsLeft, int maxMoves) {
            _moves = new Dictionary<ITileController, ICommand>();
            _turnsLeft = turnsLeft;
            _moveType = MoveTypes.Blank;
            _maxMoves = maxMoves;
            UpdateCommand();
        }

        public void SetMoveType(MoveTypes moveType) {
            _moveType = moveType;
            UpdateCommand();
        }

        private void UpdateCommand() {
            switch (_moveType) {
                case MoveTypes.Blank:
                    _command = new BlankCommand();
                    break;
                case MoveTypes.Fire:
                    _command = new SetFireCommand(_intensity);
                    break;
                case MoveTypes.Water:
                    _command = new AddWaterCommand(_intensity);
                    break;
            }
        }

        public void UpdateIntensity(int intensity) {
            _intensity = intensity;
            UpdateCommand();
        }

        public bool CanTakeAction() { return HasTurnsLeft() && _moves.Count < _maxMoves; }

        public bool HasQueuedActions() { return _moves.Count > 0; }

        public int GetTurnsLeft() { return _turnsLeft; }

        public bool HasTurnsLeft() { return _turnsLeft > 0; }

        public MoveTypes GetMoveType() { return _moveType; }

        public void ProcessAction(ITileController tileController) {
            _moves.Remove(tileController);
            if (_moveType != MoveTypes.Blank && CanTakeAction() 
                    && _command.CanBeExecutedOnTile(tileController)) {
                _moves.Add(tileController, _command);
            }
        }

        public void UndoAllActions() { _moves = new Dictionary<ITileController, ICommand>(); }

        public void ClearTile(ITileController tileController) { _moves.Remove(tileController); }

        public IDictionary<ITileController, ICommand> GetAndResetMoves() {
            _turnsLeft--;
            _turnsLeft = Math.Max(0, _turnsLeft);
            IDictionary<ITileController, ICommand> moveCopy = new Dictionary<ITileController, ICommand>(_moves);
            _moves = new Dictionary<ITileController, ICommand>();
            return moveCopy;
        }
    }
}
