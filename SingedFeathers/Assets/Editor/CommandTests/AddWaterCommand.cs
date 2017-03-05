using Assets.Scripts.Controllers;
using NUnit.Framework;
using NSubstitute;
using Assets.Scripts.Models.Commands;
using Assets.Scripts.Models;

namespace Assets.Editor.CommandTests {
    [TestFixture]
    public class AddWaterCommandTest {
        private AddWaterCommand _waterCommand;
        private ITileController _tile;

        [SetUp]
        public void Init() { _tile = Substitute.For<ITileController>(); }

        [Test]
        public void TestReturnsCorrectMoveType() {
            _waterCommand = new AddWaterCommand(0);
            Assert.AreEqual(MoveTypes.Water, _waterCommand.GetMoveType());
        }

        [Test]
        public void TestExecuteCommand() {
            _waterCommand = new AddWaterCommand(1);
            _waterCommand.ExecuteCommand(_tile);
            _tile.Received().ReduceHeat(1);

            _waterCommand = new AddWaterCommand(10);
            _waterCommand.ExecuteCommand(_tile);
            _tile.Received().ReduceHeat(10);
        }

        [Test]
        public void TestHeatCannotBeNegative() {
            _waterCommand = new AddWaterCommand(0);
            _waterCommand.ExecuteCommand(_tile);
            _tile.Received().ReduceHeat(0);

            _waterCommand = new AddWaterCommand(-1);
            _waterCommand.ExecuteCommand(_tile);
            _tile.Received().ReduceHeat(0);
        }

        [Test]
        public void TestCanBeExecutedOnTile() {
            _waterCommand = new AddWaterCommand(0);

            _tile.IsFlammable().Returns(true);
            _tile.HasPositiveHeat().Returns(true);
            Assert.True(_waterCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(false);
            _tile.HasPositiveHeat().Returns(true);
            Assert.False(_waterCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(false);
            _tile.HasPositiveHeat().Returns(false);
            Assert.False(_waterCommand.CanBeExecutedOnTile(_tile));

            _tile.IsFlammable().Returns(true);
            _tile.HasPositiveHeat().Returns(false);
            Assert.False(_waterCommand.CanBeExecutedOnTile(_tile));
        }
    }
}