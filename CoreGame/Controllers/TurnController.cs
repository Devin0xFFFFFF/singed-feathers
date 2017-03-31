using System;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.Commands;

namespace CoreGame.Controllers {
    public class TurnController : ITurnController {
        private MoveType _moveType;
        private ICommand _command;
        private const int _INTENSITY = 100;
        private int _turnsLeft;
        private Delta _delta;

        public TurnController(int turnsLeft) {
            _delta = null;
            _turnsLeft = turnsLeft;
            _moveType = MoveType.None;
            UpdateCommand();
        }

        public void SetMoveType(MoveType moveType) {
            _moveType = moveType;
            UpdateCommand();
        }
        
        public bool HasQueuedAction() { return _delta != null; }

        public int GetTurnsLeft() { return _turnsLeft; }

        public bool HasTurnsLeft() { return _turnsLeft > 0; }

        public MoveType GetMoveType() { return _moveType; }

        public bool ProcessAction(ITileController tileController) {
            if (_command.CanBeExecutedOnTile(tileController)) {
                _delta = new Delta(tileController.Position, _command);
                return true;
            }
            return false;
        }

        public string GetExecutionFailureReason(ITileController tileController) { return _command.GetExecutionFailureReason(tileController); }

        public void UndoAction() { _delta = null; }
        
        public Delta GetAndResetMove() {
            _turnsLeft = Math.Max(0, _turnsLeft - 1);
            Delta deltaCopy = _delta;
            _delta = null;
            return deltaCopy;
        }

        private void UpdateCommand() {
            switch (_moveType) {
                case MoveType.Fire:
                    _command = new Command(MoveType.Fire, _INTENSITY);
                    break;
                case MoveType.Water:
                    _command = new Command(MoveType.Water, _INTENSITY);
                    break;
            }
        }
    }
}
