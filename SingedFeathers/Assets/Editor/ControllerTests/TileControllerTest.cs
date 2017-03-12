using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using NUnit.Framework;

namespace Assets.Editor.ControllerTests {
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
            Assert.False(_tileController.IsSpreadingHeat());

            _tileController = new TileController(TileType.Stone, 0, 0);
            Assert.AreEqual(TileType.Stone, _tileController.GetTileType());
            Assert.False(_tileController.IsFlammable());
            Assert.False(_tileController.IsBurntOut());
            Assert.False(_tileController.IsSpreadingHeat());
            Assert.False(_tileController.IsOnFire());

            _tileController = new TileController(TileType.Ash, 0, 0);
            Assert.AreEqual(TileType.Ash, _tileController.GetTileType());
            Assert.False(_tileController.IsFlammable());
            Assert.True(_tileController.IsBurntOut());
            Assert.False(_tileController.IsSpreadingHeat());
            Assert.False(_tileController.IsOnFire());

            _tileController = new TileController(TileType.Wood, 0, 0);
            Assert.AreEqual(TileType.Wood, _tileController.GetTileType());
            Assert.True(_tileController.IsFlammable());
            Assert.False(_tileController.IsBurntOut());
            Assert.False(_tileController.IsOnFire());
            Assert.False(_tileController.IsSpreadingHeat());

        }

        [Test]
        public void TestChangingOnFireStatus() {
            Assert.AreEqual(false, _tileController.IsOnFire());
            _tileController.ApplyHeat(100);
            Assert.AreEqual(true, _tileController.IsOnFire());
            _tileController.Extinguish();
            Assert.AreEqual(false, _tileController.IsOnFire());
        }

        [Test]
        public void TestChangingWaterStatus() {
            Assert.AreEqual(false, _tileController.IsOnFire());
            _tileController.ApplyHeat(100);
            Assert.AreEqual(true, _tileController.IsOnFire());
            _tileController.ReduceHeat(100);
            Assert.AreEqual(false, _tileController.IsOnFire());
        }

        [Test]
        public void TestHeatCannotBeReducedBelowZero() {
            Assert.AreEqual(false, _tileController.IsOnFire());
            _tileController.ReduceHeat(100);
            Assert.AreEqual(false, _tileController.IsOnFire());
            _tileController.ApplyHeat(100);
            Assert.AreEqual(true, _tileController.IsOnFire());
        }

        [Test]
        public void TestTileUpdatesAfterBurnout() {
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());
            Assert.AreEqual(false, _tileController.IsOnFire());
            Assert.AreEqual(false, _tileController.IsBurntOut());

            // Light the tile and spread once
            _tileController.ApplyHeat(100);
            _tileController.SpreadFire();
            Assert.AreEqual(false, _tileController.IsBurntOut());
            Assert.AreEqual(true, _tileController.IsOnFire());
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());

            // Spread again -- still not burnt out
            _tileController.SpreadFire();
            Assert.AreEqual(false, _tileController.IsBurntOut());
            Assert.AreEqual(true, _tileController.IsOnFire());
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());

            // Spread again -- burnt out
            _tileController.SpreadFire();
            Assert.AreEqual(true, _tileController.IsBurntOut());
            Assert.AreEqual(TileType.Ash, _tileController.GetTileType());
        }

        [Test]
        public void TestTileTypeUpdatesAfterBurnsOut() {
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());

            //Ignite tile and burn it out
            _tileController.ApplyHeat(100);
            Assert.True(_tileController.IsOnFire());

            _tileController.SpreadFire();
            _tileController.SpreadFire();
            _tileController.SpreadFire();

            Assert.AreEqual(TileType.Ash, _tileController.GetTileType());
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
            woodNeighbour.StateHasChanged = false;
            Assert.True(woodNeighbour.IsOnFire());
            Assert.True(woodNeighbour.IsSpreadingHeat());
            _tileController.AddNeighbouringTile(woodNeighbour);

            Assert.False(_tileController.IsOnFire());
            Assert.False(_tileController.StateHasChanged);

            _tileController.SpreadFire();

            Assert.True(_tileController.IsOnFire());
            Assert.True(_tileController.StateHasChanged);

            _tileController.SpreadFire();

            Assert.True(_tileController.IsOnFire());
            Assert.True(_tileController.StateHasChanged);
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
    }
}