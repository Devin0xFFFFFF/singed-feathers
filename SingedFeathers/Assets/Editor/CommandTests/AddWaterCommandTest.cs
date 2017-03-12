using Assets.Scripts.Controllers;
using NUnit.Framework;
using NSubstitute;
using Assets.Scripts.Models.Commands;
using Assets.Scripts.Models;

namespace Assets.Editor.CommandTests {
    [TestFixture]
    public class AddWaterCommandTest {
        private Command _waterCommand;
        private ITileController _tile;

        [SetUp]
        public void Init() { _tile = Substitute.For<ITileController>(); }

        [Test]
        public void TestReturnsCorrectMoveType() {
            _waterCommand = new Command(MoveType.Water, 0);
            Assert.AreEqual(MoveType.Water, _waterCommand.MoveType);
        }

        [Test]
        public void TestExecuteCommand() {
            _waterCommand = new Command(MoveType.Water, 1);
            _waterCommand.ExecuteCommand(_tile);
            _tile.Received().ReduceHeat(1);

            _waterCommand = new Command(MoveType.Water, 10);
            _waterCommand.ExecuteCommand(_tile);
            _tile.Received().ReduceHeat(10);
        }

        [Test]
        public void TestHeatCannotBeNegative() {
            _waterCommand = new Command(MoveType.Water, 0);
            _waterCommand.ExecuteCommand(_tile);
            _tile.Received().ReduceHeat(0);

            _waterCommand = new Command(MoveType.Water, -1);
            _waterCommand.ExecuteCommand(_tile);
            _tile.Received().ReduceHeat(0);
        }

        [Test]
        public void TestCanBeExecutedOnTile() {
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
            Assert.False(_waterCommand.CanBeExecutedOnTile(_tile));
        }
    }
}