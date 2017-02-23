using NUnit.Framework;

namespace SingedFeathers.Test.ControllerTest {
    [TestFixture]
    public class TileControllerTest {
        private TileController _tileController;

        [SetUp]
        public void Init() {
            _tileController = new TileController(TileType.Grass);
        }

        [Test]
        public void TestInitializingTile() {
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());
            Assert.False(_tileController.IsBurntOut());
            Assert.False(_tileController.IsOnFire());
        }

        [Test]
        public void TestChangingLitStatus() {
            Assert.AreEqual(false, _tileController.IsOnFire());
            _tileController.ApplyHeat(100);
            Assert.AreEqual(true, _tileController.IsOnFire());
            _tileController.Extinguish();
            Assert.AreEqual(false, _tileController.IsOnFire());
        }

        [Test]
        public void TestTileUpdatesAfterBurnout() {
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());
            Assert.AreEqual(false, _tileController.IsOnFire());
            Assert.AreEqual(false, _tileController.IsBurntOut());

            //light the tile and spread once
            _tileController.ApplyHeat(100);
            _tileController.SpreadFire();
            Assert.AreEqual(false, _tileController.IsBurntOut());
            Assert.AreEqual(true, _tileController.IsOnFire());
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());

            //spread again -- still not burnt out
            _tileController.SpreadFire();
            Assert.AreEqual(false, _tileController.IsBurntOut());
            Assert.AreEqual(true, _tileController.IsOnFire());
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());

            //spread again -- burnt out
            _tileController.SpreadFire();
            Assert.AreEqual(true, _tileController.IsBurntOut());
            Assert.AreEqual(TileType.Ash, _tileController.GetTileType());
        }

        [Test]
        public void TestTileTypeUpdatesAfterBurnsOut() {
            Assert.AreEqual(TileType.Grass, _tileController.GetTileType());

            //ignite tile and burn it out
            _tileController.ApplyHeat(100);
            _tileController.IsOnFire();
            _tileController.SpreadFire();
            _tileController.SpreadFire();
            _tileController.SpreadFire();

            Assert.AreEqual(TileType.Ash, _tileController.GetTileType());
        }
    }
}
