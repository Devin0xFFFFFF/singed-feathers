using Assets.Scripts.Models;
using Assets.Scripts.Models.Commands;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Controllers {
    public class TurnController : ITurnController {
        private MoveType _moveType;
        private ICommand _command;
        private const int _INTENSITY = 100;
        private int _maxMoves;
        private int _turnsLeft;
        private IDictionary<ITileController, ICommand> _moves;

        public TurnController(int turnsLeft, int maxMoves) {
            _moves = new Dictionary<ITileController, ICommand>();
            _turnsLeft = turnsLeft;
            _moveType = MoveType.Remove;
            _maxMoves = maxMoves;
            UpdateCommand();
        }

        public void SetMoveType(MoveType moveType) {
            _moveType = moveType;
            UpdateCommand();
        }

        public bool CanTakeAction() { return HasTurnsLeft() && _moves.Count < _maxMoves; }

        public bool HasQueuedActions() { return _moves.Count > 0; }

        public int GetTurnsLeft() { return _turnsLeft; }

        public bool HasTurnsLeft() { return _turnsLeft > 0; }

        public MoveType GetMoveType() { return _moveType; }

        public void ProcessAction(ITileController tileController) {
            _moves.Remove(tileController);
            if (_moveType != MoveType.Remove && CanTakeAction() 
                    && _command.CanBeExecutedOnTile(tileController)) {
                _moves.Add(tileController, _command);
            }
        }

        public void UndoAllActions() { _moves = new Dictionary<ITileController, ICommand>(); }

        public void ClearTile(ITileController tileController) { _moves.Remove(tileController); }

        public IDictionary<ITileController, ICommand> GetAndResetMoves() {
            _turnsLeft = Math.Max(0, _turnsLeft - 1);
            IDictionary<ITileController, ICommand> moveCopy = new Dictionary<ITileController, ICommand>(_moves);
            _moves = new Dictionary<ITileController, ICommand>();
            return moveCopy;
        }

		private void UpdateCommand() {
			switch (_moveType) {
			    case MoveType.Remove:
				    _command = new Command(MoveType.Remove);
				    break;
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
