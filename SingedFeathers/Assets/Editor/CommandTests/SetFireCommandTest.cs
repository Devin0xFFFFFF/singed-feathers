using Assets.Scripts.Controllers;
using NUnit.Framework;
using NSubstitute;
using Assets.Scripts.Models.Commands;
using Assets.Scripts.Models;

namespace Assets.Editor.CommandTests {
    [TestFixture]
    public class SetFireCommandTest {
        private Command _fireCommand;
        private ITileController _tile;

        [SetUp]
        public void Init() { _tile = Substitute.For<ITileController>(); }

        [Test]
        public void TestReturnsCorrectMoveType() {
            _fireCommand = new Command(MoveType.Fire, 0);
            Assert.AreEqual(MoveType.Fire, _fireCommand.MoveType);
        }

        [Test]
        public void TestExecuteCommand() {
            _fireCommand = new Command(MoveType.Fire, 1);
            _fireCommand.ExecuteCommand(_tile);
            _tile.Received().ApplyHeat(1);

            _fireCommand = new Command(MoveType.Fire, 10);
            _fireCommand.ExecuteCommand(_tile);
            _tile.Received().ApplyHeat(10);
        }

        [Test]
        public void TestHeatCannotBeNegative() {
            _fireCommand = new Command(MoveType.Fire, 0);
            _fireCommand.ExecuteCommand(_tile);
            _tile.Received().ApplyHeat(0);

            _fireCommand = new Command(MoveType.Fire, -1);
            _fireCommand.ExecuteCommand(_tile);
            _tile.Received().ApplyHeat(0);
        }

        [Test]
        public void TestCanBeExecutedOnTile() {
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
    }
}