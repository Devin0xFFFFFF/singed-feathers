using Assets.Scripts.Models;
using Assets.Scripts.Models.Commands;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    class TurnController : ITurnController {
        private MoveTypes _moveType;
        private ICommand command;
        private int _intensity = 100;
        private int _maxMoves = 1;
        private int _turnsLeft;
        private IDictionary<ITileController, ICommand> moves;

        public TurnController(int turnsLeft) {
            moves = new Dictionary<ITileController, ICommand>();
            _turnsLeft = turnsLeft;
            _moveType = MoveTypes.Blank;
            UpdateCommand();
        }

        public void SetMoveType(MoveTypes moveType) {
            _moveType = moveType;
            UpdateCommand();
        }

        private void UpdateCommand() {
            switch (_moveType) {
                case MoveTypes.Blank:
                    command = new BlankCommand();
                    break;
                case MoveTypes.Fire:
                    command = new SetFireCommand(_intensity);
                    break;
                case MoveTypes.Water:
                    command = new AddWaterCommand(_intensity);
                    break;
            }
        }

        public void UpdateIntensity(int intensity) {
            _intensity = intensity;
            UpdateCommand();
        }

        public bool CanTakeAction() {
            return HasTurnsLeft() && moves.Count < _maxMoves;
        }

        public bool HasQueuedActions() {
            return moves.Count < 0;
        }

        public int GetTurnsLeft() {
            return _turnsLeft;
        }

        public bool HasTurnsLeft() {
            return _turnsLeft > 0;
        }

        public MoveTypes GetMoveType() {
            return _moveType;
        }

        public void ProcessAction(ITileController tileController) {
            if (_moveType == MoveTypes.Blank) {
                moves.Remove(tileController);
            } else if (CanTakeAction() && command.CanBeExecutedOnTile(tileController)) {
                moves.Add(tileController, command);
            }
        }

        public void UndoAllActions() {
            moves = new Dictionary<ITileController, ICommand>();
        }

        public void ClearTile(ITileController tileController) {
            moves.Remove(tileController);
        }

        public IDictionary<ITileController, ICommand> GetAndResetMoves() {
            _turnsLeft--;
            _turnsLeft = Math.Max(0, _turnsLeft);
            IDictionary<ITileController, ICommand> moveCopy = new Dictionary<ITileController, ICommand>(moves);
            moves = new Dictionary<ITileController, ICommand>();
            return moveCopy;
        }
    }
}
