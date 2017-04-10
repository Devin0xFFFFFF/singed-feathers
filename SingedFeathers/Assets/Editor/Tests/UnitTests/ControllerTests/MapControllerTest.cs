using System;
using System.Collections.Generic;
using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Service;
using CoreGame.Utility;
using NUnit.Framework;
using NSubstitute;

namespace Assets.Editor.Tests.UnitTests.ControllerTests {
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
            MapLocationValidator.InitializeValues(testMap);
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
            Assert.AreEqual("You lose! No pigeons survived!", _mapController.GetGameOverPlayerStatus());

            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual("You win! No pigeons survived!", _mapController.GetGameOverPlayerStatus());

            // Test with at least one live pigeon
            _pigeon1.IsDead().Returns(false);

            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual("You win! A pigeon survived!", _mapController.GetGameOverPlayerStatus());

            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual("You lose! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
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
        public void TestReduceHeatAffectsExpectedTileAtValidLocation() {
            _mapController.ReduceHeat(0, 0);
            _tile0.Received().ReduceHeat(Arg.Any<int>());

            _mapController.ReduceHeat(0, 1);
            _tile1.Received().ReduceHeat(Arg.Any<int>());

            _mapController.ReduceHeat(0, 2);
            _tile2.Received().ReduceHeat(Arg.Any<int>());
        }

        [Test]
        public void TestReduceHeatThrowsNoExceptionAtInvalidLocation() {
            try {
                // Both values too large
                _mapController.ReduceHeat(10, 10);
                _tile0.DidNotReceive().ReduceHeat(Arg.Any<int>());
                _tile1.DidNotReceive().ReduceHeat(Arg.Any<int>());
                _tile2.DidNotReceive().ReduceHeat(Arg.Any<int>());

                // Both values negative
                _mapController.ReduceHeat(-2, -4);
                _tile0.DidNotReceive().ReduceHeat(Arg.Any<int>());
                _tile1.DidNotReceive().ReduceHeat(Arg.Any<int>());
                _tile2.DidNotReceive().ReduceHeat(Arg.Any<int>());

                // X value negative
                _mapController.ReduceHeat(-10, 20);
                _tile0.DidNotReceive().ReduceHeat(Arg.Any<int>());
                _tile1.DidNotReceive().ReduceHeat(Arg.Any<int>());
                _tile2.DidNotReceive().ReduceHeat(Arg.Any<int>());

                // Y value negative
                _mapController.ReduceHeat(20, -15);
                _tile0.DidNotReceive().ReduceHeat(Arg.Any<int>());
                _tile1.DidNotReceive().ReduceHeat(Arg.Any<int>());
                _tile2.DidNotReceive().ReduceHeat(Arg.Any<int>());

                // X value valid
                _mapController.ReduceHeat(0, 11);
                _tile0.DidNotReceive().ReduceHeat(Arg.Any<int>());
                _tile1.DidNotReceive().ReduceHeat(Arg.Any<int>());
                _tile2.DidNotReceive().ReduceHeat(Arg.Any<int>());

                // X value valid
                _mapController.ReduceHeat(-12, 0);
            } catch (Exception e) {
                Assert.Fail("Expected no exception, but got {0}", e.Message);
            }
        }

        [Test]
        public void TestGetTileTypeReturnsTypeOfTileAtValidLocation() {
            TileType type00 = _mapController.GetTileType(0, 0);
            Assert.AreEqual(TileType.Grass, type00);

            TileType type01 = _mapController.GetTileType(0, 1);
            Assert.AreEqual(TileType.Grass, type01);

            TileType type02 = _mapController.GetTileType(0, 2);
            Assert.AreEqual(TileType.Stone, type02);
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
        public void TestGetTileControllerReturnsControllerAtSpecifiedLocation() {
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
            _turnResolver.Received().ResolveTurn(Arg.Any<Delta>(), Arg.Any<Map>(), Arg.Any<Player>());
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
        public void TestIsGameOver() {
            _pigeon0.IsDead().Returns(false);
            _pigeon1.IsDead().Returns(false);

            _tile0.IsFlammable().Returns(false);
            _tile0.IsBurntOut().Returns(false);

            _tile1.IsFlammable().Returns(true);
            _tile1.IsBurntOut().Returns(true);

            _tile2.IsFlammable().Returns(true);
            _tile2.IsBurntOut().Returns(false);

            _turnController.HasTurnsLeft().Returns(true);

            Assert.False(_mapController.IsGameOver());

            _pigeon1.IsDead().Returns(true);
            Assert.False(_mapController.IsGameOver());

            _pigeon0.IsDead().Returns(true);
            Assert.True(_mapController.IsGameOver());

            _tile2.IsBurntOut().Returns(true);
            Assert.True(_mapController.IsGameOver());

            _turnController.HasTurnsLeft().Returns(false);
            Assert.True(_mapController.IsGameOver());

            _turnController.HasTurnsLeft().Returns(true);
            Assert.True(_mapController.IsGameOver());

            _pigeon0.IsDead().Returns(false);
            _pigeon1.IsDead().Returns(false);
            Assert.True(_mapController.IsGameOver());

            _tile2.IsBurntOut().Returns(false);
            Assert.False(_mapController.IsGameOver());

            _turnController.HasTurnsLeft().Returns(false);
            Assert.True(_mapController.IsGameOver());
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

        [Test]
        public void TestGetLivePigeonCount() {
            _pigeon0.IsDead().Returns(false);
            _pigeon1.IsDead().Returns(false);
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());

            _pigeon0.IsDead().Returns(true);
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());

            _pigeon1.IsDead().Returns(true);
            Assert.AreEqual(0, _mapController.GetLivePigeonCount());

            // Test empty map with no pigeons
            Assert.AreEqual(0, _emptyMapController.GetLivePigeonCount());
        }

        [Test]
        public void TestGenerateDefaultMap() {
            MapController mc = new MapController();
            Assert.IsTrue(mc.GenerateDefaultMap());
        }

        [Test]
        public void TestUpdateTileController() {
            Assert.False(_mapController.UpdateTileController(TileType.Stone, -2, -1));
            Assert.False(_mapController.UpdateTileController(TileType.Stone, 2, -1));
            Assert.False(_mapController.UpdateTileController(TileType.Stone, -2, 1));

            Assert.AreEqual(TileType.Grass, _mapController.GetTileType(0, 0));
            Assert.True(_mapController.UpdateTileController(TileType.Wood, 0, 0));
            Assert.AreEqual(TileType.Wood, _mapController.GetTileType(0, 0));
        }

        [Test]
        public void TestAddInitialPigeonPosition() {
            // Invalid positions return false
            Assert.False(_mapController.AddInitialPigeonPosition(new Position(12, 5)));
            Assert.False(_mapController.AddInitialPigeonPosition(new Position(-4, 5)));

            // Can add to unoccupied tile
            Assert.True(_mapController.AddInitialPigeonPosition(new Position(1, 0)));

            // Cannot add to occupied tile
            Assert.False(_mapController.AddInitialPigeonPosition(new Position(0, 0)));
            Assert.False(_mapController.AddInitialPigeonPosition(new Position(1, 0)));
        }

        [Test]
        public void TestRemoveInitialPigeonPosition() {
            // Invalid positions return false
            Assert.False(_mapController.RemoveInitialPigeonPosition(new Position(12, 5)));
            Assert.False(_mapController.RemoveInitialPigeonPosition(new Position(-4, 5)));

            // Cannot remove if no pigeon is there
            Assert.False(_mapController.RemoveInitialPigeonPosition(new Position(1, 1)));
            Assert.False(_mapController.RemoveInitialPigeonPosition(new Position(1, 0)));

            // Can remove if pigeon in space indicated
            Assert.True(_mapController.RemoveInitialPigeonPosition(new Position(0, 0)));
            Assert.True(_mapController.RemoveInitialPigeonPosition(new Position(0, 1)));
        }

        [Test]
        public void TestAddInitialFirePosition() {
            // Invalid positions return false
            Assert.False(_mapController.AddInitialFirePosition(new Position(12, 5)));
            Assert.False(_mapController.AddInitialFirePosition(new Position(-4, 5)));

            // Can ignite flammable tire but not if it's on fire
            Assert.False(_mapController.AddInitialFirePosition(new Position(0, 2)));
            
            // Can ignite flammable tile that is not on fire
            _tile2.IsOnFire().Returns(false);
            Assert.False(_mapController.AddInitialFirePosition(new Position(0, 2)));

            // Cannot add to non-flammable tile or tile that is already on fire
            Assert.False(_mapController.AddInitialFirePosition(new Position(0, 0)));
            Assert.False(_mapController.AddInitialFirePosition(new Position(1, 0)));
        }

        [Test]
        public void TestRemoveInitialFirePosition() {
            // Invalid positions return false
            Assert.False(_mapController.RemoveInitialFirePosition(new Position(12, 5)));
            Assert.False(_mapController.RemoveInitialFirePosition(new Position(-4, 5)));

            Assert.True(_mapController.RemoveInitialFirePosition(new Position(1, 1)));
            Assert.True(_mapController.RemoveInitialFirePosition(new Position(1, 0)));

            Assert.False(_mapController.RemoveInitialFirePosition(new Position(0, 0)));
            Assert.False(_mapController.RemoveInitialFirePosition(new Position(0, 1)));
        }

        [Test]
        public void TestUpdateNumberOfTurns() {
            // Can only update with a valid number of turns
            Assert.False(_mapController.UpdateNumberOfTurns(30));
            Assert.False(_mapController.UpdateNumberOfTurns(-10));
            Assert.False(_mapController.UpdateNumberOfTurns(2));
            Assert.False(_mapController.UpdateNumberOfTurns(26));

            Assert.True(_mapController.UpdateNumberOfTurns(13));
            Assert.True(_mapController.UpdateNumberOfTurns(9));
            Assert.True(_mapController.UpdateNumberOfTurns(5));
            Assert.True(_mapController.UpdateNumberOfTurns(20));
        }

        private Map GenerateTestMap() {
            ITileController[,] tiles = IntializeTileControllers();
            int width = 2;
            int height = 3;
            return new Map() {
                Height = height,
                Width = width,
                InitialFirePositions = new List<Position>() { new Position(1, 0), new Position(1, 1) },
                InitialPigeonPositions = new List<Position>() { new Position(0, 0), new Position(0, 1) },
                TileMap = tiles,
                RawMap = InitializeRawMap(tiles, width, height),
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
                InitialFirePositions = new List<Position>() {},
                InitialPigeonPositions = new List<Position>() {},
                TileMap = tiles,
                RawMap = new TileType[0,0],
                Pigeons = new List<IPigeonController>() { },
                TurnController = turnController,
                TurnResolver = turnResolver
            };
        }

        private ITileController[,] IntializeTileControllers() {
            _tile0 = Substitute.For<ITileController>();
            _tile0.GetTileType().Returns(TileType.Grass);
            _tile0.IsOnFire().Returns(true);
            _tile0.IsBurntOut().Returns(false);
            _tile0.IsFlammable().Returns(false);

            _tile1 = Substitute.For<ITileController>();
            _tile1.GetTileType().Returns(TileType.Grass);
            _tile1.IsOnFire().Returns(true);
            _tile1.IsBurntOut().Returns(false);

            _tile2 = Substitute.For<ITileController>();
            _tile2.GetTileType().Returns(TileType.Stone);
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

        private TileType[,] InitializeRawMap(ITileController[,] tiles, int width, int height) {
            TileType[,] rawMap = new TileType[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    rawMap[x, y] = tiles[x, y].GetTileType();
                }
            }
            return rawMap;
        }
    }
}