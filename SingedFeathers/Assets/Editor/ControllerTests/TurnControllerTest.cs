using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using Assets.Scripts.Service;
using NUnit.Framework;
using NSubstitute;
using Assets.Scripts.Models.Commands;
using System.Collections;

namespace Assets.Editor.ControllerTests {
    [TestFixture]
    public class TurnControllerTest {
        private TurnController _turnController;
        private ITileController _tile0;
        private ITileController _tile1;
        private ITileController _tile2;
        private ITileController _tile3;

        [SetUp]
        public void Init() { InitializeTiles(); }

        [Test]
        public void TestTurnControllerInitializesProperly() {
            _turnController = new TurnController(1,1);
            Assert.True(_turnController.CanTakeAction());
            Assert.False(_turnController.HasQueuedActions());
            Assert.AreEqual(1, _turnController.GetTurnsLeft());
            Assert.True(_turnController.HasTurnsLeft());
            Assert.AreEqual(MoveTypes.Remove, _turnController.GetMoveType());
        }

        [Test]
        public void TestFireInitializesProperly() {
            _turnController = new TurnController(1,1);
            _turnController.SetMoveType(MoveTypes.Fire);
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());
        }

        [Test]
        public void TestWaterInitializesProperly() {
            _turnController = new TurnController(1,1);
            _turnController.SetMoveType(MoveTypes.Water);
            Assert.AreEqual(MoveTypes.Water, _turnController.GetMoveType());
        }

        [Test]
        public void TestCancelInitializesProperly() {
            _turnController = new TurnController(1,1);
            _turnController.SetMoveType(MoveTypes.Remove);
            Assert.AreEqual(MoveTypes.Remove, _turnController.GetMoveType());
        }

        [Test]
        public void TestSingleCommandLimit() {
            _turnController = new TurnController(10, 1);
            _turnController.SetMoveType(MoveTypes.Fire);
            _turnController.ProcessAction(_tile0);
            Assert.False(_turnController.CanTakeAction());
            Assert.True(_turnController.HasQueuedActions());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());
        }

        [Test]
        public void TestUndoForSingleCommand() {
            _turnController = new TurnController(10, 1);
            _turnController.SetMoveType(MoveTypes.Fire);
            _turnController.ProcessAction(_tile0);
            _turnController.UndoAllActions();
            Assert.True(_turnController.CanTakeAction());
            Assert.False(_turnController.HasQueuedActions());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());
        }

        [Test]
        public void TestMultipleCommandLimit() {
            _turnController = new TurnController(10, 3);
            _turnController.SetMoveType(MoveTypes.Fire);
            _turnController.ProcessAction(_tile0);
            _turnController.ProcessAction(_tile1);
            Assert.True(_turnController.CanTakeAction());
            Assert.True(_turnController.HasQueuedActions());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());
            _turnController.ProcessAction(_tile2);
            Assert.False(_turnController.CanTakeAction());
            Assert.True(_turnController.HasQueuedActions());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());
        }

        [Test]
        public void TestUndoForMultipleCommands() {
            _turnController = new TurnController(10, 3);
            _turnController.SetMoveType(MoveTypes.Fire);
            _turnController.ProcessAction(_tile0);
            _turnController.ProcessAction(_tile1);
            _turnController.ProcessAction(_tile2);
            _turnController.UndoAllActions();
            Assert.True(_turnController.CanTakeAction());
            Assert.False(_turnController.HasQueuedActions());
            Assert.AreEqual(10, _turnController.GetTurnsLeft());
            Assert.True(_turnController.HasTurnsLeft());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());
        }

        [Test]
        public void TestClearTile() {
            _turnController = new TurnController(10, 3);
            _turnController.SetMoveType(MoveTypes.Fire);
            _turnController.ProcessAction(_tile0);
            _turnController.ProcessAction(_tile1);
            _turnController.ProcessAction(_tile2);
            Assert.False(_turnController.CanTakeAction());
            Assert.True(_turnController.HasQueuedActions());
            Assert.AreEqual(10, _turnController.GetTurnsLeft());
            Assert.True(_turnController.HasTurnsLeft());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());

            _turnController.ClearTile(_tile0);
            Assert.True(_turnController.CanTakeAction());
            Assert.True(_turnController.HasQueuedActions());
            Assert.AreEqual(10, _turnController.GetTurnsLeft());
            Assert.True(_turnController.HasTurnsLeft());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());

            _turnController.ClearTile(_tile1);
            Assert.True(_turnController.CanTakeAction());
            Assert.True(_turnController.HasQueuedActions());
            Assert.AreEqual(10, _turnController.GetTurnsLeft());
            Assert.True(_turnController.HasTurnsLeft());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());

            _turnController.ClearTile(_tile2);
            Assert.True(_turnController.CanTakeAction());
            Assert.False(_turnController.HasQueuedActions());
            Assert.AreEqual(10, _turnController.GetTurnsLeft());
            Assert.True(_turnController.HasTurnsLeft());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());
        }

        [Test]
        public void TestGetAndResetForMultipleCommands() {
            _turnController = new TurnController(10, 3);
            _turnController.SetMoveType(MoveTypes.Fire);
            _turnController.ProcessAction(_tile0);
            _turnController.SetMoveType(MoveTypes.Water);
            _turnController.ProcessAction(_tile1);
            _turnController.SetMoveType(MoveTypes.Fire);
            _turnController.ProcessAction(_tile2);
            Assert.False(_turnController.CanTakeAction());
            Assert.True(_turnController.HasQueuedActions());
            Assert.AreEqual(10, _turnController.GetTurnsLeft());
            Assert.True(_turnController.HasTurnsLeft());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());

            IDictionary<ITileController, ICommand> moves = _turnController.GetAndResetMoves();
            Assert.True(_turnController.CanTakeAction());
            Assert.False(_turnController.HasQueuedActions());
            Assert.AreEqual(9, _turnController.GetTurnsLeft());
            Assert.True(_turnController.HasTurnsLeft());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());

            Assert.AreEqual(3, moves.Count);
            Assert.AreEqual(moves[_tile0].GetMoveType(), MoveTypes.Fire);
            Assert.AreEqual(moves[_tile1].GetMoveType(), MoveTypes.Water);
            Assert.AreEqual(moves[_tile2].GetMoveType(), MoveTypes.Fire);
        }

        [Test]
        public void TestGetAndResetForMoreCommandsThenMax() {
            _turnController = new TurnController(10, 2);
            _turnController.SetMoveType(MoveTypes.Fire);
            _turnController.ProcessAction(_tile0);
            _turnController.SetMoveType(MoveTypes.Water);
            _turnController.ProcessAction(_tile1);
            _turnController.SetMoveType(MoveTypes.Fire);
            _turnController.ProcessAction(_tile2);
            Assert.False(_turnController.CanTakeAction());
            Assert.True(_turnController.HasQueuedActions());
            Assert.AreEqual(10, _turnController.GetTurnsLeft());
            Assert.True(_turnController.HasTurnsLeft());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());

            IDictionary<ITileController, ICommand> moves = _turnController.GetAndResetMoves();
            Assert.True(_turnController.CanTakeAction());
            Assert.False(_turnController.HasQueuedActions());
            Assert.AreEqual(9, _turnController.GetTurnsLeft());
            Assert.True(_turnController.HasTurnsLeft());
            Assert.AreEqual(MoveTypes.Fire, _turnController.GetMoveType());

            Assert.AreEqual(2, moves.Count);
            Assert.AreEqual(moves[_tile0].GetMoveType(), MoveTypes.Fire);
            Assert.AreEqual(moves[_tile1].GetMoveType(), MoveTypes.Water);
            Assert.False(moves.ContainsKey(_tile2));
        }


        [Test]
        public void TestLastTurn() {
            _turnController = new TurnController(1, 2);
            Assert.True(_turnController.CanTakeAction());
            Assert.False(_turnController.HasQueuedActions());
            Assert.AreEqual(1, _turnController.GetTurnsLeft());
            Assert.True(_turnController.HasTurnsLeft());
            Assert.AreEqual(MoveTypes.Remove, _turnController.GetMoveType());

            IDictionary<ITileController, ICommand> moves = _turnController.GetAndResetMoves();
            Assert.False(_turnController.CanTakeAction());
            Assert.False(_turnController.HasQueuedActions());
            Assert.AreEqual(0, _turnController.GetTurnsLeft());
            Assert.False(_turnController.HasTurnsLeft());
            Assert.AreEqual(MoveTypes.Remove, _turnController.GetMoveType());

            Assert.AreEqual(0, moves.Count);
        }

        private void InitializeTiles() {
            _tile0 = Substitute.For<ITileController>();
            _tile0.IsFlammable().Returns(true);
            _tile0.HasPositiveHeat().Returns(true);
            _tile0.IsOccupied.Returns(false);

            _tile1 = Substitute.For<ITileController>();
            _tile1.IsFlammable().Returns(true);
            _tile1.HasPositiveHeat().Returns(true);
            _tile1.IsOccupied.Returns(false);

            _tile2 = Substitute.For<ITileController>();
            _tile2.IsFlammable().Returns(true);
            _tile2.HasPositiveHeat().Returns(true);
            _tile2.IsOccupied.Returns(false);

            _tile3 = Substitute.For<ITileController>();
            _tile3.IsFlammable().Returns(true);
            _tile3.HasPositiveHeat().Returns(true);
            _tile3.IsOccupied.Returns(false);
        }
    }
}