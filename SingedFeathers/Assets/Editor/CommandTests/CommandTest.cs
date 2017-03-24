using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.Commands;
using NUnit.Framework;
using NSubstitute;

namespace Assets.Editor.CommandTests {
    [TestFixture]
    public class CommandTest {
        private Command _fireCommand;
        private Command _waterCommand;
        private ITileController _tile;

        [SetUp]
        public void Init() { _tile = Substitute.For<ITileController>(); }

        [Test]
        public void TestReturnsCorrectMoveType() {
            _fireCommand = new Command(MoveType.Fire, 0);
            Assert.AreEqual(MoveType.Fire, _fireCommand.MoveType);

            _waterCommand = new Command(MoveType.Water, 0);
            Assert.AreEqual(MoveType.Water, _waterCommand.MoveType);
        }

        [Test]
        public void TestExecuteFireCommand() {
            _fireCommand = new Command(MoveType.Fire, 1);
            _fireCommand.ExecuteCommand(_tile);
            _tile.Received().ApplyHeat(1);

            _fireCommand = new Command(MoveType.Fire, 10);
            _fireCommand.ExecuteCommand(_tile);
            _tile.Received().ApplyHeat(10);
        }

        [Test]
        public void TestExecuteWaterCommand() {
            _waterCommand = new Command(MoveType.Water, 1);
            _waterCommand.ExecuteCommand(_tile);
            _tile.Received().ReduceHeat(1);

            _waterCommand = new Command(MoveType.Water, 10);
            _waterCommand.ExecuteCommand(_tile);
            _tile.Received().ReduceHeat(10);
        }

        [Test]
        public void TestHeatCannotBeNegative() {
            _fireCommand = new Command(MoveType.Fire, 0);
            _fireCommand.ExecuteCommand(_tile);
            _tile.Received().ApplyHeat(0);

            _fireCommand = new Command(MoveType.Fire, -1);
            _fireCommand.ExecuteCommand(_tile);
            _tile.Received().ApplyHeat(0);

            _waterCommand = new Command(MoveType.Water, 0);
            _waterCommand.ExecuteCommand(_tile);
            _tile.Received().ReduceHeat(0);

            _waterCommand = new Command(MoveType.Water, -1);
            _waterCommand.ExecuteCommand(_tile);
            _tile.Received().ReduceHeat(0);
        }

        [Test]
        public void TestCanBeExecutedOnTileIfMoveTypeIsFire() {
            _fireCommand = new Command(MoveType.Fire, 0);

            _tile.IsFlammable().Returns(true);
            _tile.IsOccupied.Returns(true);
            _tile.IsOnFire().Returns(true);
            Assert.False(_fireCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(false);
            _tile.IsOccupied.Returns(true);
            _tile.IsOnFire().Returns(true);
            Assert.False(_fireCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(false);
            _tile.IsOccupied.Returns(false);
            _tile.IsOnFire().Returns(true);
            Assert.False(_fireCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(true);
            _tile.IsOccupied.Returns(false);
            _tile.IsOnFire().Returns(true);
            Assert.False(_fireCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(true);
            _tile.IsOccupied.Returns(true);
            _tile.IsOnFire().Returns(false);
            Assert.False(_fireCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(false);
            _tile.IsOccupied.Returns(true);
            _tile.IsOnFire().Returns(false);
            Assert.False(_fireCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(false);
            _tile.IsOccupied.Returns(false);
            _tile.IsOnFire().Returns(false);
            Assert.False(_fireCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(true);
            _tile.IsOccupied.Returns(false);
            _tile.IsOnFire().Returns(false);
            Assert.True(_fireCommand.CanBeExecutedOnTile(_tile));
        }

        [Test]
        public void TestCanBeExecutedOnTileIfMoveTypeIsWater() {
            _waterCommand = new Command(MoveType.Water, 0);

            _tile.IsFlammable().Returns(true);
            _tile.IsHeatZero().Returns(false);
            Assert.True(_waterCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(false);
            _tile.IsHeatZero().Returns(false);
            Assert.False(_waterCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(false);
            _tile.IsHeatZero().Returns(true);
            Assert.False(_waterCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(true);
            _tile.IsHeatZero().Returns(true);
            Assert.True(_waterCommand.CanBeExecutedOnTile(_tile));
        }

		[Test]
		public void TestGetExecutionFailureReasonIfMoveTypeIsFire() {
			_fireCommand = new Command(MoveType.Fire, 0);

			_tile.IsFlammable().Returns(true);
			_tile.IsOccupied.Returns(true);
			_tile.IsOnFire().Returns(true);
			Assert.AreEqual("Move not allowed! This tile is occupied.", _fireCommand.GetExecutionFailureReason(_tile));

			_tile.IsFlammable().Returns(false);
			_tile.IsOccupied.Returns(true);
			_tile.IsOnFire().Returns(true);
			Assert.AreEqual("Move not allowed! This tile is not flammable.", _fireCommand.GetExecutionFailureReason(_tile));

			_tile.IsFlammable().Returns(false);
			_tile.IsOccupied.Returns(false);
			_tile.IsOnFire().Returns(true);
			Assert.AreEqual("Move not allowed! This tile is not flammable.", _fireCommand.GetExecutionFailureReason(_tile));

			_tile.IsFlammable().Returns(true);
			_tile.IsOccupied.Returns(false);
			_tile.IsOnFire().Returns(true);
			Assert.AreEqual("Move not allowed! This tile is already on fire.", _fireCommand.GetExecutionFailureReason(_tile));

			_tile.IsFlammable().Returns(true);
			_tile.IsOccupied.Returns(true);
			_tile.IsOnFire().Returns(false);
			Assert.AreEqual("Move not allowed! This tile is occupied.", _fireCommand.GetExecutionFailureReason(_tile));

			_tile.IsFlammable().Returns(false);
			_tile.IsOccupied.Returns(true);
			_tile.IsOnFire().Returns(false);
			Assert.AreEqual("Move not allowed! This tile is not flammable.", _fireCommand.GetExecutionFailureReason(_tile));

			_tile.IsFlammable().Returns(false);
			_tile.IsOccupied.Returns(false);
			_tile.IsOnFire().Returns(false);
			Assert.AreEqual("Move not allowed! This tile is not flammable.", _fireCommand.GetExecutionFailureReason(_tile));

			_tile.IsFlammable().Returns(true);
			_tile.IsOccupied.Returns(false);
			_tile.IsOnFire().Returns(false);
			Assert.AreEqual("", _fireCommand.GetExecutionFailureReason(_tile));
		}

		[Test]
		public void TestGetExecutionFailureReasonIfMoveTypeIsWater() {
			_waterCommand = new Command(MoveType.Water, 0);

			_tile.IsFlammable().Returns(true);
			_tile.IsHeatZero().Returns(false);
			Assert.AreEqual("", _waterCommand.GetExecutionFailureReason(_tile));

			_tile.IsFlammable().Returns(false);
			_tile.IsHeatZero().Returns(false);
			Assert.AreEqual("Move not allowed! This tile cannot be on fire.", _waterCommand.GetExecutionFailureReason(_tile));

			_tile.IsFlammable().Returns(false);
			_tile.IsHeatZero().Returns(true);
			Assert.AreEqual("Move not allowed! This tile cannot be on fire.", _waterCommand.GetExecutionFailureReason(_tile));

			_tile.IsFlammable().Returns(true);
			_tile.IsHeatZero().Returns(true);
			Assert.AreEqual("", _waterCommand.GetExecutionFailureReason(_tile));
		}
    }
}