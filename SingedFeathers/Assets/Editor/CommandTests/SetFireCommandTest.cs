using Assets.Scripts.Controllers;
using NUnit.Framework;
using NSubstitute;
using Assets.Scripts.Models.Commands;
using Assets.Scripts.Models;

namespace Assets.Editor.CommandTests {
    [TestFixture]
    public class SetFireCommandTest {
        private SetFireCommand _fireCommand;
        private ITileController _tile;

        [SetUp]
        public void Init() { _tile = Substitute.For<ITileController>(); }

        [Test]
        public void TestReturnsCorrectMoveType() {
            _fireCommand = new SetFireCommand(0);
            Assert.AreEqual(MoveTypes.Fire, _fireCommand.GetMoveType());
        }

        [Test]
        public void TestExecuteCommand() {
            _fireCommand = new SetFireCommand(1);
            _fireCommand.ExecuteCommand(_tile);
            _tile.Received().ApplyHeat(1);

            _fireCommand = new SetFireCommand(10);
            _fireCommand.ExecuteCommand(_tile);
            _tile.Received().ApplyHeat(10);
        }

        [Test]
        public void TestHeatCannotBeNegative() {
            _fireCommand = new SetFireCommand(0);
            _fireCommand.ExecuteCommand(_tile);
            _tile.Received().ApplyHeat(0);

            _fireCommand = new SetFireCommand(-1);
            _fireCommand.ExecuteCommand(_tile);
            _tile.Received().ApplyHeat(0);
        }

        [Test]
        public void TestCanBeExecutedOnTile() {
            _fireCommand = new SetFireCommand(0);

            _tile.IsFlammable().Returns(true);
            _tile.IsOccupied.Returns(true);
            Assert.False(_fireCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(false);
            _tile.IsOccupied.Returns(true);
            Assert.False(_fireCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(false);
            _tile.IsOccupied.Returns(false);
            Assert.False(_fireCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(true);
            _tile.IsOccupied.Returns(false);
            Assert.True(_fireCommand.CanBeExecutedOnTile(_tile));
        }
    }
}