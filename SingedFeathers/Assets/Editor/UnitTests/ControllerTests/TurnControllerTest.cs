using NUnit.Framework;
using NSubstitute;
using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;

namespace Assets.Editor.UnitTests.ControllerTests {
    [TestFixture]
    public class TurnControllerTest {
        private TurnController _turnController;
        private ITileController _tile0;
        private ITileController _tile1;
        private ITileController _tile2;
        private ITileController _tile3;
		private ITileController _tile4;

        [SetUp]
        public void Init() { InitializeTiles(); }

        [Test]
        public void TestTurnControllerInitializesProperly() {
            _turnController = new TurnController(1);
            Assert.True(_turnController.HasTurnsLeft());
            Assert.False(_turnController.HasQueuedAction());
            Assert.AreEqual(1, _turnController.GetTurnsLeft());
            Assert.AreEqual(MoveType.Fire, _turnController.GetMoveType());
        }

        [Test]
        public void TestFireInitializesProperly() {
            _turnController = new TurnController(1);
            _turnController.SetMoveType(MoveType.Fire);
            Assert.AreEqual(MoveType.Fire, _turnController.GetMoveType());
        }

        [Test]
        public void TestWaterInitializesProperly() {
            _turnController = new TurnController(1);
            _turnController.SetMoveType(MoveType.Water);
            Assert.AreEqual(MoveType.Water, _turnController.GetMoveType());
        }

        [Test]
        public void TestFireCommandReturnsTrueOnFlammable() {
            _turnController = new TurnController(10);
            _turnController.SetMoveType(MoveType.Fire);
            Assert.True(_turnController.ProcessAction(_tile0));
        }

        [Test]
        public void TestGetExecutionFailureReasonIsBlankForAllowedActions() {
            _turnController = new TurnController(10);
            _turnController.SetMoveType(MoveType.Fire);
            Assert.AreEqual("", _turnController.GetExecutionFailureReason(_tile0));

            _turnController.SetMoveType(MoveType.Water);
            Assert.AreEqual("", _turnController.GetExecutionFailureReason(_tile0));
            Assert.AreEqual("", _turnController.GetExecutionFailureReason(_tile2));
            Assert.AreEqual("", _turnController.GetExecutionFailureReason(_tile3));
        }

        [Test]
        public void TestFireCommandReturnsFalseOnNonFlammable() {
            _turnController = new TurnController(10);
            _turnController.SetMoveType(MoveType.Fire);
            Assert.False(_turnController.ProcessAction(_tile4));
        }

        [Test]
        public void TestGetExecutionFailureReasonForDisallowedActions() {
            _turnController = new TurnController(10);
            _turnController.SetMoveType(MoveType.Fire);
            Assert.AreEqual("Move not allowed! This tile is not flammable.", _turnController.GetExecutionFailureReason(_tile4));
            Assert.AreEqual("Move not allowed! This tile is already burnt out.", _turnController.GetExecutionFailureReason(_tile1));
            Assert.AreEqual("Move not allowed! This tile is occupied.", _turnController.GetExecutionFailureReason(_tile2));
            Assert.AreEqual("Move not allowed! This tile is already on fire.", _turnController.GetExecutionFailureReason(_tile3));

            _turnController.SetMoveType(MoveType.Water);
            Assert.AreEqual("Move not allowed! This tile cannot be on fire.", _turnController.GetExecutionFailureReason(_tile4));
            Assert.AreEqual("Move not allowed! This tile is already burnt out.", _turnController.GetExecutionFailureReason(_tile1));
        }

        [Test]
        public void TestWaterCommandReturnsFalseOnNoHeat() {
            _turnController = new TurnController(10);
            _turnController.SetMoveType(MoveType.Water);
            Assert.False(_turnController.ProcessAction(_tile4));
        }

        [Test]
        public void TestWaterCommandReturnsTrueOnHeat() {
            _turnController = new TurnController(10);
            _turnController.SetMoveType(MoveType.Water);
            Assert.True(_turnController.ProcessAction(_tile0));
        }

        [Test]
        public void TestSingleCommandLimit() {
            _turnController = new TurnController(10);
            _turnController.SetMoveType(MoveType.Fire);
            _turnController.ProcessAction(_tile0);
               Assert.True(_turnController.HasTurnsLeft());
            Assert.True(_turnController.HasQueuedAction());
            Assert.AreEqual(MoveType.Fire, _turnController.GetMoveType());
        }

        [Test]
        public void TestUndoForSingleCommand() {
            _turnController = new TurnController(10);
            _turnController.SetMoveType(MoveType.Fire);
            _turnController.ProcessAction(_tile0);
            _turnController.UndoAction();
            Assert.True(_turnController.HasTurnsLeft());
            Assert.False(_turnController.HasQueuedAction());
            Assert.AreEqual(MoveType.Fire, _turnController.GetMoveType());
        }
        
        [Test]
        public void TestLastTurn() {
            _turnController = new TurnController(1);
            Assert.True(_turnController.HasTurnsLeft());
            Assert.False(_turnController.HasQueuedAction());
            Assert.AreEqual(1, _turnController.GetTurnsLeft());
            Assert.AreEqual(MoveType.Fire, _turnController.GetMoveType());

            Delta move = _turnController.GetAndResetMove();
            Assert.False(_turnController.HasTurnsLeft());
            Assert.False(_turnController.HasQueuedAction());
            Assert.AreEqual(0, _turnController.GetTurnsLeft());
            Assert.AreEqual(MoveType.Fire, _turnController.GetMoveType());

            Assert.Null(move);
        }

        private void InitializeTiles() {
            _tile0 = Substitute.For<ITileController>();
            _tile0.IsFlammable().Returns(true);
            _tile0.IsHeatZero().Returns(false);
            _tile0.IsOccupied.Returns(false);

            _tile1 = Substitute.For<ITileController>();
            _tile1.IsFlammable().Returns(false);
            _tile1.IsHeatZero().Returns(false);
            _tile1.IsOccupied.Returns(false);
            _tile1.IsBurntOut().Returns(true);

            _tile2 = Substitute.For<ITileController>();
            _tile2.IsFlammable().Returns(true);
            _tile2.IsHeatZero().Returns(false);
            _tile2.IsOccupied.Returns(true);
            _tile2.IsBurntOut().Returns(false);

            _tile3 = Substitute.For<ITileController>();
            _tile3.IsFlammable().Returns(true);
            _tile3.IsHeatZero().Returns(false);
            _tile3.IsOccupied.Returns(false);
            _tile3.IsOnFire().Returns(true);

            _tile4 = Substitute.For<ITileController>();
            _tile4.IsFlammable().Returns(false);
            _tile4.IsHeatZero().Returns(true);
            _tile4.IsOccupied.Returns(false);
        }
    }
}