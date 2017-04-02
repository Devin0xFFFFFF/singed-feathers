using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using NSubstitute;
using NUnit.Framework;

namespace Assets.Editor.UnitTests.ControllerTests {
    [TestFixture]
    public class TileControllerTest {

        private TileController _tileController;

        [SetUp]
        public void Init() { _tileController = new TileController(TileType.Grass, 0, 0); }

        [Test]
        public void TestInitializingTileBasedOnType() {
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());
            Assert.True(_tileController.IsFlammable());
            Assert.False(_tileController.IsBurntOut());
            Assert.False(_tileController.IsOnFire());

            _tileController = new TileController(TileType.Stone, 0, 0);
            Assert.AreEqual(TileType.Stone, _tileController.GetTileType());
            Assert.False(_tileController.IsFlammable());
            Assert.False(_tileController.IsBurntOut());
            Assert.False(_tileController.IsOnFire());

            _tileController = new TileController(TileType.Wood, 0, 0);
            Assert.AreEqual(TileType.Wood, _tileController.GetTileType());
            Assert.True(_tileController.IsFlammable());
            Assert.False(_tileController.IsBurntOut());
            Assert.False(_tileController.IsOnFire());

        }

        [Test]
        public void TestChangingOnFireStatus() {
            Assert.AreEqual(false, _tileController.IsOnFire());
            _tileController.ApplyHeat(100);
            Assert.AreEqual(false, _tileController.IsOnFire());
            _tileController.UpKeep();
            Assert.AreEqual(true, _tileController.IsOnFire());
            _tileController.ReduceHeat(100);
            Assert.AreEqual(false, _tileController.IsOnFire());
        }

        [Test]
        public void TestHeatCannotBeReducedBelowZero() {
            Assert.AreEqual(false, _tileController.IsOnFire());
            _tileController.ReduceHeat(100);
            Assert.AreEqual(false, _tileController.IsOnFire());
            _tileController.ApplyHeat(99);
            _tileController.UpKeep();
            Assert.AreEqual(true, _tileController.IsOnFire());
        }

        [Test]
        public void TestTileBurnsOut() {
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());
            Assert.AreEqual(false, _tileController.IsOnFire());
            Assert.AreEqual(false, _tileController.IsBurntOut());

            // Light the tile and spread once
            _tileController.ApplyHeat(100);
            _tileController.UpKeep();
            Assert.AreEqual(false, _tileController.IsBurntOut());
            Assert.AreEqual(true, _tileController.IsOnFire());
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());

            _tileController.SpreadFire();
            _tileController.UpKeep();
            Assert.AreEqual(false, _tileController.IsBurntOut());
            Assert.AreEqual(true, _tileController.IsOnFire());
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());

            // Spread again -- still not burnt out
            _tileController.SpreadFire();
            _tileController.UpKeep();
            Assert.AreEqual(false, _tileController.IsBurntOut());
            Assert.AreEqual(true, _tileController.IsOnFire());
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());

            // Spread again -- burnt out
            _tileController.SpreadFire();
            _tileController.UpKeep();
            Assert.AreEqual(true, _tileController.IsBurntOut());
            Assert.AreEqual(false, _tileController.IsOnFire());
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());
        }

        [Test]
        public void TestNonFlammableTypeCannotIgnite() {
            _tileController = new TileController(TileType.Stone, 0, 0);
            Assert.False(_tileController.IsOnFire());
            Assert.False(_tileController.IsBurntOut());

            // Try to ignite it
            _tileController.ApplyHeat(10000);
            Assert.False(_tileController.IsOnFire());
            Assert.False(_tileController.IsBurntOut());
        }

        [Test]
        public void TestIncrementallyTakesHeatFromNeighbours() {
            ITileController woodNeighbour = new TileController(TileType.Wood, 0, 0);
            woodNeighbour.ApplyHeat(100);
            woodNeighbour.UpKeep();
            Assert.True(woodNeighbour.IsOnFire());
            woodNeighbour.AddNeighbouringTile(_tileController);

            Assert.False(_tileController.IsOnFire());

            woodNeighbour.SpreadFire();
            _tileController.UpKeep();

            Assert.True(_tileController.IsOnFire());
            woodNeighbour.SpreadFire();
            _tileController.UpKeep();

            Assert.True(_tileController.IsOnFire());
        }

        [Test]
        public void TestOccupyingAndLeavingTile() {
            Assert.False(_tileController.IsOccupied);
            _tileController.MarkOccupied();
            Assert.True(_tileController.IsOccupied);
            _tileController.MarkUnoccupied();
            Assert.False(_tileController.IsOccupied);    
        }

        [Test]
        public void TestIsHeatZero() {
            Assert.True(_tileController.IsHeatZero());
            _tileController.ApplyHeat(1);
            Assert.False(_tileController.IsHeatZero());
            _tileController.ReduceHeat(1);
            Assert.True(_tileController.IsHeatZero());
        }

        [Test]
        public void TestOccupyingOccupiedTile() {
            Assert.False(_tileController.IsOccupied);

            // Occupying an unoccupied tile should return true => occupy was successful
            Assert.True(_tileController.MarkOccupied());
            Assert.True(_tileController.IsOccupied);

            // Occupying an occupied tile should return false => cannot occupy occupied tile
            Assert.False(_tileController.MarkOccupied());
            Assert.True(_tileController.IsOccupied);
        }

        [Test]
        public void TestLeaveUnoccupiedTile() {
            Assert.False(_tileController.IsOccupied);

            // Leaving an unoccupied tile should return false => can't leave what isn't occupied
            Assert.False(_tileController.MarkUnoccupied());
            Assert.False(_tileController.IsOccupied);

            Assert.True(_tileController.MarkOccupied());
            Assert.True(_tileController.IsOccupied);

            // Leaving an occupied tile should return true => successfully left
            Assert.True(_tileController.MarkUnoccupied());
            Assert.False(_tileController.IsOccupied);
        }

        [Test]
        public void TestGetTileHeat() {
            // GetTileHeat should return Tile.Heat
            Assert.True(_tileController.GetTileHeat() == _tileController.Tile.Heat);
        }

        [Test]
        public void TestCompareTo() {
            ITileController other0 = Substitute.For<ITileController>();
            other0.GetTileHeat().Returns(0);
            other0.Position.Returns(new Position(0, 0));

            ITileController other1 = Substitute.For<ITileController>();
            other1.GetTileHeat().Returns(0);
            other1.Position.Returns(new Position(5, 5));

            ITileController other2 = Substitute.For<ITileController>();
            other2.GetTileHeat().Returns(1);
            other2.Position.Returns(new Position(0, 0));

            ITileController other3 = Substitute.For<ITileController>();
            other3.GetTileHeat().Returns(1);
            other3.Position.Returns(new Position(5, 5));

            ITileController other4 = Substitute.For<ITileController>();
            other4.GetTileHeat().Returns(2);
            other4.Position.Returns(new Position(0, 0));

            ITileController other5 = Substitute.For<ITileController>();
            other5.GetTileHeat().Returns(2);
            other5.Position.Returns(new Position(5, 5));

            _tileController = new TileController(TileType.Grass, 2, 2);
            _tileController.ApplyHeat(1);

            Assert.True(_tileController.CompareTo(other0) > 0);
            Assert.True(_tileController.CompareTo(other1) > 0);
            Assert.True(_tileController.CompareTo(other2) > 0);
            Assert.True(_tileController.CompareTo(other3) < 0);
            Assert.True(_tileController.CompareTo(other4) < 0);
            Assert.True(_tileController.CompareTo(other5) < 0);
        }
    }
}