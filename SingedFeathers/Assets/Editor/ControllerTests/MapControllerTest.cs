using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Service;

namespace Assets.Editor.ControllerTests {
    [TestFixture]
    public class MapControllerTest {
        private MapController _mapController;
        private MapController _emptyMapController;
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

            // Initialize empty map with no pigeons
            Map emptyMap = GenerateEmptyMap();
            mapGenerator.GenerateMap("Empty").Returns(emptyMap);
            _emptyMapController = new MapController(mapGenerator);
            _emptyMapController.GenerateMap("Empty");

            Map testMap = GenerateTestMap();
            mapGenerator.GenerateMap("TestMap").Returns(testMap);
            _mapController = new MapController(mapGenerator);
            _mapController.GenerateMap("TestMap");
        }

        [Test]
        public void TestGenerateInitializesProperly() {
            Assert.AreEqual(3, _mapController.Height);
            Assert.AreEqual(2, _mapController.Width);
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());
            Assert.AreEqual(2, _mapController.GetPigeonControllers().Count);
            Assert.True(_mapController.GetTileController(0, 0).IsOnFire());
            Assert.True(_mapController.GetTileController(0, 1).IsOnFire());

            Assert.AreEqual(0, _emptyMapController.Height);
            Assert.AreEqual(0, _emptyMapController.Width);
        }

        [Test]
        public void TestSetAndGetPlayerSideSelection() {
            _mapController.SetPlayerSideSelection(0);
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());

            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());

            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual(PlayerSideSelection.BurnPigeons, _mapController.GetPlayerSideSelection());
        }

        [Test]
        public void TestGetGameOverPlayerStatus() {
            // Test with all dead pigeons
            _pigeon0.IsDead().Returns(true);
            _pigeon1.IsDead().Returns(true);

            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual("You lost! No pigeons survived!", _mapController.GetGameOverPlayerStatus());

            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual("You won! No pigeons survived!", _mapController.GetGameOverPlayerStatus());

            // Test with at least one live pigeon
            _pigeon1.IsDead().Returns(false);

            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual("You won! A pigeon survived!", _mapController.GetGameOverPlayerStatus());

            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual("You lost! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
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
                Assert.Fail("Expected no exception, but got {0}", e.Message);
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
                Assert.Fail("Expected no exception, but got {0}", e.Message);
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
                Assert.Fail("Expected no exception, but got {0}", e.Message);
            }
        }

        [Test]
        public void TestGetPigeonControllersReturnsExpectedPigeons() {
            IList<IPigeonController> pigeons = _mapController.GetPigeonControllers();
            Assert.AreEqual(pigeons[0], _pigeon0);
            Assert.AreEqual(pigeons[1], _pigeon1);
        }

        [Test]
        public void TestEndTurnMethod() {
            _mapController.EndTurn();
            _turnController.Received().GetAndResetMove();
            _turnResolver.Received().ResolveTurn(Arg.Any<Delta>(), Arg.Any<Map>());
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

        [Test]
        public void TestIsMapBurntOut() {
            _tile0.IsFlammable().Returns(false);
            _tile0.IsBurntOut().Returns(false);

            _tile1.IsFlammable().Returns(true);
            _tile1.IsBurntOut().Returns(true);

            _tile2.IsFlammable().Returns(true);
            _tile2.IsBurntOut().Returns(false);

            Assert.IsFalse(_mapController.IsMapBurntOut());

            _tile2.IsBurntOut().Returns(true);
            Assert.IsTrue(_mapController.IsMapBurntOut());

            // Test empty map
            Assert.IsTrue(_emptyMapController.IsMapBurntOut());
        }

        [Test]
        public void TestAreAllPigeonsDead() {
            _pigeon0.IsDead().Returns(false);
            _pigeon1.IsDead().Returns(false);
            Assert.IsFalse(_mapController.AreAllPigeonsDead());

            _pigeon0.IsDead().Returns(true);
            Assert.IsFalse(_mapController.AreAllPigeonsDead());

            _pigeon1.IsDead().Returns(true);
            Assert.IsTrue(_mapController.AreAllPigeonsDead());

            // Test empty map with no pigeons
            Assert.IsTrue(_emptyMapController.AreAllPigeonsDead());
        }

        private Map GenerateTestMap() {
            return new Map() {
                Height = 3,
                Width = 2,
                InitialFirePositions = new List<Position>() { new Position(1, 0), new Position(1, 1) },
                InitialPigeonPositions = new List<Position>() { new Position(0, 0), new Position(0, 1) },
                TileMap = IntializeTileControllers(),
                Pigeons = InitializePigeons(),
                TurnController = InitializeTurnController(),
                TurnResolver = InitializeTurnResolver()
            };
        }

        private Map GenerateEmptyMap() {
            ITileController[,] tiles = { {} };
            ITurnController turnController = Substitute.For<ITurnController>();
            ITurnResolver turnResolver = Substitute.For<ITurnResolver>();
            return new Map() {
                Height = 0,
                Width = 0,
                InitialFirePositions = new List<Position> () {},
                InitialPigeonPositions = new List<Position>() {},
                TileMap = tiles,
                Pigeons = new List<IPigeonController>() {},
                TurnController = turnController,
                TurnResolver = turnResolver
            };
        }

        private ITileController[,] IntializeTileControllers() {
            _tile0 = Substitute.For<ITileController>();
            _tile0.GetTileType().Returns(TileType.Stone);
            _tile0.IsOnFire().Returns(true);
            _tile0.IsBurntOut().Returns(false);

            _tile1 = Substitute.For<ITileController>();
            _tile1.GetTileType().Returns(TileType.Grass);
            _tile1.IsOnFire().Returns(true);
            _tile1.IsBurntOut().Returns(false);

            _tile2 = Substitute.For<ITileController>();
            _tile2.GetTileType().Returns(TileType.Wood);
            _tile2.IsOnFire().Returns(false);
            _tile2.IsBurntOut().Returns(true);

            ITileController[,] tiles = {
                { _tile0, _tile1, _tile2 },
                { Substitute.For<ITileController>(), Substitute.For<ITileController>(), Substitute.For<ITileController>() }
            };
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