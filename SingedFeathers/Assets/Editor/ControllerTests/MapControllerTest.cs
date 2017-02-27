using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using Assets.Scripts.Service;
using NUnit.Framework;
using NSubstitute;

namespace Assets.Editor.ControllerTests {
    [TestFixture()]
    public class MapControllerTest {
        private MapController _mapController;
        private ITileController _tile0;
        private ITileController _tile1;
        private ITileController _tile2;

        [SetUp]
        public void Init() {
            IMapGeneratorService mapGenerator = Substitute.For<IMapGeneratorService>();
            Map testMap = GenerateTestMap();
            mapGenerator.GenerateMap(Arg.Any<int>()).Returns(testMap);
            _mapController = new MapController(mapGenerator);
            _mapController.GenerateMap();
        }

        [Test]
        public void TestGenerateInitializesProperly() {
            Assert.AreEqual(3, _mapController.Height);
            Assert.AreEqual(1, _mapController.Width);
        }

        [Test]
        public void TestApplyHeatAffectsTileAtValidLocation() {
            _mapController.ApplyHeat(0, 0);
            _tile0.Received().ApplyHeat(Arg.Any<int>());

            _mapController.ApplyHeat(0, 1);
            _tile1.Received().ApplyHeat(Arg.Any<int>());

            _mapController.ApplyHeat(0, 2);
            _tile2.Received().ApplyHeat(Arg.Any<int>());
        }

        [Test]
        public void TestApplyHeatThrowsNoExceptionAtInvalidLocation() {
            try {
                // Both values too large
                _mapController.ApplyHeat(10, 10);

                // Both values negative
                _mapController.ApplyHeat(-2, -4);

                // X value negative
                _mapController.ApplyHeat(-10, 20);

                // Y value negative
                _mapController.ApplyHeat(20, -15);

                // X value valid
                _mapController.ApplyHeat(0, 11);

                // X value valid
                _mapController.ApplyHeat(-12, 0);
            } catch (Exception e) {
                Assert.Fail(string.Format("Expected no exception, but got {0}", e.Message));
            }
        }

        [Test]
        public void TestGetTileTypeReturnsTypeOfTileAtValidLocation() {
            TileType type00 = _mapController.GetTileType(0, 0);
            Assert.AreEqual(TileType.Stone, type00);

            TileType type01 = _mapController.GetTileType(0, 1);
            Assert.AreEqual(TileType.Grass, type01);

            TileType type02 = _mapController.GetTileType(0, 2);
            Assert.AreEqual(TileType.Wood, type02);
        }

        [Test]
        public void TestNoExceptionThrownIfInvalidLocationSpecifiedForGetTileType() {
            try {
                TileType type;
                // Both values too large
                type = _mapController.GetTileType(10, 10);
                Assert.AreEqual(TileType.Error, type);

                // Both values negative
                type = _mapController.GetTileType(-2, -4);
                Assert.AreEqual(TileType.Error, type);

                // X value negative
                type = _mapController.GetTileType(-10, 20);
                Assert.AreEqual(TileType.Error, type);

                // Y value negative
                type = _mapController.GetTileType(4, -12);
                Assert.AreEqual(TileType.Error, type);

                // X value valid
                type = _mapController.GetTileType(0, 11);
                Assert.AreEqual(TileType.Error, type);

                // Y value value
                type = _mapController.GetTileType(-12, 0);
                Assert.AreEqual(TileType.Error, type);
            }
            catch (Exception e) {
                Assert.Fail(string.Format("Expected no exception, but got {0}", e.Message));
            }
        }

        [Test]
        public void TestGetControllerReturnsControllerAtSpecifiedLocation() {
            ITileController controller00 = _mapController.GetController(0, 0);
            Assert.AreEqual(controller00, _tile0);

            ITileController controller01 = _mapController.GetController(0, 1);
            Assert.AreEqual(controller01, _tile1);

            ITileController controller02 = _mapController.GetController(0, 2);
            Assert.AreEqual(controller02, _tile2);
        }

        [Test]
        public void TestNoExceptionThrownIfInvalidLocationSpecifiedForGetController() {
            try {
                ITileController controller;
                // Both values too large
                controller = _mapController.GetController(10, 10);
                Assert.Null(controller);

                // Both values negative
                controller = _mapController.GetController(-2, -4);
                Assert.Null(controller);

                // X value negative
                controller = _mapController.GetController(-10, 20);
                Assert.Null(controller);

                // Y value negative
                controller = _mapController.GetController(20, -15);
                Assert.Null(controller);

                // X value valid
                controller = _mapController.GetController(0, 11);
                Assert.Null(controller);

                // Y value value
                controller = _mapController.GetController(-12, 0);
                Assert.Null(controller);
            } catch (Exception e) {
                Assert.Fail(string.Format("Expected no exception, but got {0}", e.Message));
            }
        }

        [Test]
        public void TestSpreadFiresReturnsEmptyDictionaryIfNoTilesHaveChanged() {
            // Mark tiles as not having been changed
            _tile0.StateHasChanged.Returns(false);
            _tile1.StateHasChanged.Returns(false);
            _tile2.StateHasChanged.Returns(false);

            IDictionary<NewStatus, IList<Position>> modifiedTiles = _mapController.SpreadFires();
            
            Assert.NotNull(modifiedTiles);
            foreach (IList<Position> tilesOfNewStatus in modifiedTiles.Values) {
                Assert.False(tilesOfNewStatus.Any());
            }
        }

        [Test]
        public void TestSpreadFiresRetursExpectedDictionaryForChangedTiles() {
            IDictionary<NewStatus, IList<Position>> modifiedTiles = _mapController.SpreadFires();
            Assert.NotNull(modifiedTiles);

            IList<Position> tilesNowOnFire = modifiedTiles[NewStatus.OnFire];
            Assert.True(tilesNowOnFire.Any());
            Assert.AreEqual(2, tilesNowOnFire.Count);

            IList<Position> tilesNowBurntOut = modifiedTiles[NewStatus.BurntOut];
            Assert.True(tilesNowBurntOut.Any());
            Assert.AreEqual(1, tilesNowBurntOut.Count);
        }

        private Map GenerateTestMap() {
            ITileController[,] tileControllers = IntializeControllers();
            return new Map() {
                Height = 3,
                Width = 1,
                InitialFirePosition = new Position() { X = 1, Y = 0 },
                TileMap = tileControllers
            };
        }

        private ITileController[,] IntializeControllers() {
            _tile0 = Substitute.For<ITileController>();
            _tile0.GetTileType().Returns(TileType.Stone);
            _tile0.StateHasChanged.Returns(true);
            _tile0.IsOnFire().Returns(true);
            _tile0.IsBurntOut().Returns(false);

            _tile1 = Substitute.For<ITileController>();
            _tile1.GetTileType().Returns(TileType.Grass);
            _tile1.StateHasChanged.Returns(true);
            _tile1.IsOnFire().Returns(true);
            _tile1.IsBurntOut().Returns(false);

            _tile2 = Substitute.For<ITileController>();
            _tile2.GetTileType().Returns(TileType.Wood);
            _tile2.StateHasChanged.Returns(true);
            _tile2.IsOnFire().Returns(false);
            _tile2.IsBurntOut().Returns(true);

            ITileController[,] tileControllers = {
                { _tile0, _tile1, _tile2 }
            };
            return tileControllers;
        }
    }
}