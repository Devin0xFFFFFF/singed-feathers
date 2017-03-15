using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.Commands;
using CoreGame.Service;

namespace Assets.Editor.ControllerTests {
    [TestFixture]
    public class MapControllerTest {
        private MapController _mapController;
        private IPigeonController _pigeon0;
        private IPigeonController _pigeon1;
        private ITileController _tile0;
        private ITileController _tile1;
        private ITileController _tile2;
        private ITurnController _turnController;
        private ITurnResolver _turnResolver;

        [SetUp]
        public void Init() {
            IMapGeneratorService mapGenerator = Substitute.For<IMapGeneratorService>();
            Map testMap = GenerateTestMap();
            mapGenerator.GenerateMap(Arg.Any<string>()).Returns(testMap);
            _mapController = new MapController(mapGenerator);
            _mapController.GenerateMap(Arg.Any<string>());
        }

        [Test]
        public void TestGenerateInitializesProperly() {
            Assert.AreEqual(3, _mapController.Height);
            Assert.AreEqual(1, _mapController.Width);
        }

        [Test]
        public void TestApplyHeatAffectsExpectedTileAtValidLocation() {
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
                _tile0.DidNotReceive().ApplyHeat(Arg.Any<int>());
                _tile1.DidNotReceive().ApplyHeat(Arg.Any<int>());
                _tile2.DidNotReceive().ApplyHeat(Arg.Any<int>());

                // Both values negative
                _mapController.ApplyHeat(-2, -4);
                _tile0.DidNotReceive().ApplyHeat(Arg.Any<int>());
                _tile1.DidNotReceive().ApplyHeat(Arg.Any<int>());
                _tile2.DidNotReceive().ApplyHeat(Arg.Any<int>());

                // X value negative
                _mapController.ApplyHeat(-10, 20);
                _tile0.DidNotReceive().ApplyHeat(Arg.Any<int>());
                _tile1.DidNotReceive().ApplyHeat(Arg.Any<int>());
                _tile2.DidNotReceive().ApplyHeat(Arg.Any<int>());

                // Y value negative
                _mapController.ApplyHeat(20, -15);
                _tile0.DidNotReceive().ApplyHeat(Arg.Any<int>());
                _tile1.DidNotReceive().ApplyHeat(Arg.Any<int>());
                _tile2.DidNotReceive().ApplyHeat(Arg.Any<int>());

                // X value valid
                _mapController.ApplyHeat(0, 11);
                _tile0.DidNotReceive().ApplyHeat(Arg.Any<int>());
                _tile1.DidNotReceive().ApplyHeat(Arg.Any<int>());
                _tile2.DidNotReceive().ApplyHeat(Arg.Any<int>());

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
            ITileController controller00 = _mapController.GetTileController(0, 0);
            Assert.AreEqual(controller00, _tile0);

            ITileController controller01 = _mapController.GetTileController(0, 1);
            Assert.AreEqual(controller01, _tile1);

            ITileController controller02 = _mapController.GetTileController(0, 2);
            Assert.AreEqual(controller02, _tile2);
        }

        [Test]
        public void TestNoExceptionThrownIfInvalidLocationSpecifiedForGetController() {
            try {
                ITileController controller;
                // Both values too large
                controller = _mapController.GetTileController(10, 10);
                Assert.Null(controller);

                // Both values negative
                controller = _mapController.GetTileController(-2, -4);
                Assert.Null(controller);

                // X value negative
                controller = _mapController.GetTileController(-10, 20);
                Assert.Null(controller);

                // Y value negative
                controller = _mapController.GetTileController(20, -15);
                Assert.Null(controller);

                // X value valid
                controller = _mapController.GetTileController(0, 11);
                Assert.Null(controller);

                // Y value value
                controller = _mapController.GetTileController(-12, 0);
                Assert.Null(controller);
            } catch (Exception e) {
                Assert.Fail(string.Format("Expected no exception, but got {0}", e.Message));
            }
        }

        [Test]
        public void TestModifiedTilePositionsReturnsEmptyDictionaryIfNoTilesHaveChanged() {
            // Mark tiles as not having been changed
            _tile0.StateHasChanged.Returns(false);
            _tile1.StateHasChanged.Returns(false);
            _tile2.StateHasChanged.Returns(false);

            _mapController.SpreadFires();

            IDictionary<NewStatus, IList<Position>> modifiedTiles = _mapController.ModifiedTilePositions;
            
            Assert.NotNull(modifiedTiles);
            foreach (IList<Position> tilesOfNewStatus in modifiedTiles.Values) {
                Assert.False(tilesOfNewStatus.Any());
            }
        }

        [Test]
        public void TestModifiedTilePositionsRetursExpectedDictionaryForChangedTiles() {
            IDictionary<NewStatus, IList<Position>> modifiedTiles = _mapController.ModifiedTilePositions;
            Assert.NotNull(modifiedTiles);

            IList<Position> tilesNowOnFire = modifiedTiles[NewStatus.OnFire];
            Assert.True(tilesNowOnFire.Any());
            Assert.AreEqual(2, tilesNowOnFire.Count);

            IList<Position> tilesNowBurntOut = modifiedTiles[NewStatus.BurntOut];
            Assert.True(tilesNowBurntOut.Any());
            Assert.AreEqual(1, tilesNowBurntOut.Count);
        }

        [Test]
        public void TestGetPigeonControllersReturnsExpectedPigeons() {
            IList<IPigeonController> pigeons = _mapController.GetPigeonControllers();
            Assert.AreEqual(pigeons[0], _pigeon0);
            Assert.AreEqual(pigeons[1], _pigeon1);
        }

        [Test]
        public void TestAllPigeonsCallReactEvenIfDead() {
            _pigeon0.IsDead().Returns(true);
            _pigeon0.Move().Returns(true); // If invoked, return true
            _pigeon1.IsDead().Returns(false);
            _pigeon1.Move().Returns(true); // If invoked, return true

            _mapController.MovePigeons();

            // Pigeon0 React() invoked
            _pigeon0.Received().React();

            // Pigeon1 React() invoked
            _pigeon1.Received().React();
        }

        [Test]
        public void TestEndTurnMethod() {
            _mapController.EndTurn();
            _turnController.Received().GetAndResetMoves();
            _turnResolver.Received().ResolveTurn(Arg.Any<IDictionary<ITileController, ICommand>>(), Arg.Any<ITileController[,]>());
        }

        [Test]
        public void TestGetTurnsLeft() {
            _turnController.GetTurnsLeft().Returns(57);
            Assert.AreEqual(57, _mapController.GetTurnsLeft());
            _turnController.Received().GetTurnsLeft();
        }

        [Test]
        public void TestFire() {
            _mapController.Fire();
            _turnController.Received().SetMoveType(MoveType.Fire);
        }

        [Test]
        public void TestWater() {
            _mapController.Water();
            _turnController.Received().SetMoveType(MoveType.Water);
        }
        
        [Test]
        public void TestGenerateMap() {
            MapController mc = new MapController();
            Assert.IsFalse(mc.GenerateMap(null));
            Assert.IsFalse(mc.GenerateMap("{"));
        }

        private Map GenerateTestMap() {
            return new Map() {
                Height = 3,
                Width = 1,
                InitialFirePosition = new Position(1, 0),
                InitialPigeonPositions = new List<Position>() { new Position(0, 0), new Position(0, 1) },
                TileMap = IntializeControllers(),
                Pigeons = InitializePigeons(),
                TurnController = InitializeTurnController(),
                TurnResolver = InitializeTurnResolver()
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

            ITileController[,] tiles = { { _tile0, _tile1, _tile2 } };
            return tiles;
        }

        private IList<IPigeonController> InitializePigeons() {
            _pigeon0 = Substitute.For<IPigeonController>();
            _pigeon1 = Substitute.For<IPigeonController>();

            IList<IPigeonController> pigeons = new List<IPigeonController>() { _pigeon0, _pigeon1 };
            return pigeons;
        }

        private ITurnController InitializeTurnController() {
            _turnController = Substitute.For<ITurnController>();
            return _turnController;
        }

        private ITurnResolver InitializeTurnResolver() {
            _turnResolver = Substitute.For<ITurnResolver>();
            return _turnResolver;
        }
    }
}