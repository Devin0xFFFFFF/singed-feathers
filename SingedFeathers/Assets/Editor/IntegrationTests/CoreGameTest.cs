using NUnit.Framework;
using CoreGame.Controllers.Interfaces;
using CoreGame.Controllers;
using CoreGame.Models;

namespace Assets.Editor.IntegrationTests {
    [TestFixture]
    public class CoreGameTest {
        private const string SERIALIZED_MAP = "{'TurnsLeft': 10, 'MaxMovesPerTurn': 1, 'Width': 8, 'Height': 8, 'InitialFirePositions': [{'X': 2,'Y': 3}, {'X': 6,'Y': 1}], 'InitialPigeonPositions': [{'X': 3, 'Y': 1}, {'X': 6, 'Y': 6}], 'RawMap': [[3, 2, 2, 2, 2, 1, 3, 1], [3, 2, 1, 2, 2, 3, 3, 2], [2, 2, 2, 2, 2, 2, 2, 1], [1, 2, 2, 1, 2, 1, 3, 2], [1, 2, 2, 2, 1, 1, 3, 2], [1, 2, 2, 2, 2, 1, 3, 1], [3, 2, 1, 2, 2, 3, 3, 2], [3, 2, 1, 2, 1, 2, 2, 1]]}";
        private const string SERIALIZED_STONE_MAP_WITH_ONE_FLAMMABLE_TILE = "{'TurnsLeft': 10, 'MaxMovesPerTurn': 1, 'Width': 4, 'Height': 4, 'InitialFirePositions': [{'X': 3,'Y': 3}], 'InitialPigeonPositions': [{'X': 1, 'Y': 1}], 'RawMap': [[1, 1, 1, 1], [1, 1, 1, 1], [1, 1, 1, 1], [1, 1, 1, 2]]}";
        private const string SERIALIZED_STONE_MAP = "{'TurnsLeft': 10, 'MaxMovesPerTurn': 1, 'Width': 4, 'Height': 4, 'InitialFirePositions': [{'X': 2,'Y': 3}, {'X': 0,'Y': 1}], 'InitialPigeonPositions': [{'X': 1, 'Y': 1}], 'RawMap': [[1, 1, 1, 1], [1, 1, 1, 1], [1, 1, 1, 1], [1, 1, 1, 1]]}";
        private const string SERIALIZED_MAP_WITH_NO_PIGEONS = "{'TurnsLeft': 10, 'MaxMovesPerTurn': 1, 'Width': 4, 'Height': 4, 'InitialFirePositions': [{'X': 2,'Y': 3}], 'InitialPigeonPositions': [], 'RawMap': [[1, 2, 2, 1], [1, 2, 2, 1], [1, 2, 2, 1], [1, 2, 2, 1]]}";
        private const string SERIALIZED_MAP_WITH_NO_INITIAL_FIRE = "{'TurnsLeft': 6, 'MaxMovesPerTurn': 1, 'Width': 4, 'Height': 4, 'InitialFirePositions': [], 'InitialPigeonPositions': [{'X': 0,'Y': 0}], 'RawMap': [[2, 2, 2, 2], [2, 2, 2, 2], [2, 2, 2, 2], [2, 2, 2, 2]]}";
        private IMapController _mapController;
        private ITurnResolver _turnResolver;

        [SetUp]
        public void Init() {
            _mapController = new MapController(); 
            _turnResolver = new LocalTurnResolver();
        }

        [Test]
        public void TestMapGeneratesProperly() {
            GenerateMap(SERIALIZED_MAP);

            // Test Map was generated properly
            Assert.AreEqual(8, _mapController.Width);
            Assert.AreEqual(8, _mapController.Height);
            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsGameOver());
        }

        [Test]
        public void PlayThroughSavingPigeonsJustHittingEndTurn() {
            GenerateMap(SERIALIZED_MAP);
            ITurnController turnController = _mapController.GetTurnController();
            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());

            // User sets water and then changes their mind
            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            turnController.SetMoveType(MoveType.Water);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(2, 3)));
            turnController.UndoAction();
            _mapController.EndTurn();

            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(4, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(3, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(2, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(1, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsGameOver());
            _mapController.EndTurn();

            // End of game
            Assert.AreEqual(0, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You win! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }
            
        [Test]
        public void PlayThroughBurningPigeonsJustHittingEndTurn() {
            GenerateMap(SERIALIZED_MAP);
            ITurnController turnController = _mapController.GetTurnController();
            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual(PlayerSideSelection.BurnPigeons, _mapController.GetPlayerSideSelection());

            // User sets fire and then changes their mind
            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            turnController.SetMoveType(MoveType.Fire);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(4, 1)));
            turnController.UndoAction();
            _mapController.EndTurn();

            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(4, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(3, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(2, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(1, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsGameOver());
            _mapController.EndTurn();

            // Game is over
            Assert.AreEqual(0, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You lose! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughWhileTryingToSavePigeons() {
            GenerateMap(SERIALIZED_MAP);
            ITurnController turnController = _mapController.GetTurnController();
            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            turnController.SetMoveType(MoveType.Water);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(2, 3)));
            _mapController.EndTurn();

            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            turnController.SetMoveType(MoveType.Water);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(5, 1)));
            _mapController.EndTurn();

            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            turnController.SetMoveType(MoveType.Water);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(6, 1)));
            _mapController.EndTurn();

            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            turnController.SetMoveType(MoveType.Water);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(5, 1)));
            _mapController.EndTurn();

            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(4, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(3, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(2, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(1, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsGameOver());
            _mapController.EndTurn();

            // End of game
            Assert.AreEqual(0, _mapController.GetTurnsLeft());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You win! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughKillingOnePigeonWhileTryingToBurnPigeons() {
            GenerateMap(SERIALIZED_MAP);
            ITurnController turnController = _mapController.GetTurnController();

            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual(PlayerSideSelection.BurnPigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            turnController.SetMoveType(MoveType.Fire);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(4, 1)));
            _mapController.EndTurn();

            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            // First Pigeon Dies
            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(4, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(3, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(2, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            _mapController.EndTurn();

            Assert.AreEqual(1, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsGameOver());
            _mapController.EndTurn();

            // End of game
            Assert.AreEqual(0, _mapController.GetTurnsLeft());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You lose! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughKillingAllPigeonsWhileTryingToBurnPigeons() {
            GenerateMap(SERIALIZED_MAP);
            ITurnController turnController = _mapController.GetTurnController();

            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual(PlayerSideSelection.BurnPigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            turnController.SetMoveType(MoveType.Fire);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(4, 1)));
            _mapController.EndTurn();

            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            turnController.SetMoveType(MoveType.Fire);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(6, 7)));
            _mapController.EndTurn();

            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            turnController.SetMoveType(MoveType.Fire);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(6, 6)));
            _mapController.EndTurn();

            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            turnController.SetMoveType(MoveType.Fire);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(6, 3)));
            _mapController.EndTurn();

            // First Pigeon Dies
            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            turnController.SetMoveType(MoveType.Fire);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(6, 4)));
            _mapController.EndTurn();

            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsGameOver());
            _mapController.EndTurn();

            // Second Pigeon Dies and Game Ends
            Assert.AreEqual(4, _mapController.GetTurnsLeft());
            Assert.AreEqual(0, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.True(_mapController.AreAllPigeonsDead());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You win! No pigeons survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughMapThatBurnsOutWhileSavingPigeons() {
            GenerateMap(SERIALIZED_STONE_MAP_WITH_ONE_FLAMMABLE_TILE);
            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            _mapController.EndTurn();

            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            _mapController.EndTurn();

            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.IsGameOver());
            _mapController.EndTurn();

            // Map burns out and Game Ends
            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.True(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You win! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughMapThatBurnsOutWhileBurningPigeons() {
            GenerateMap(SERIALIZED_STONE_MAP_WITH_ONE_FLAMMABLE_TILE);
            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual(PlayerSideSelection.BurnPigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            _mapController.EndTurn();

            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            _mapController.EndTurn();

            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.IsGameOver());
            _mapController.EndTurn();

            // Map burns out and Game Ends
            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.True(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You lose! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughStoneMapWhileSavingPigeons() {
            GenerateMap(SERIALIZED_STONE_MAP);
            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());

            // Game automatically ends because there are no flammable tiles
            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.True(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You win! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughStoneMapWhileBurningPigeons() {
            GenerateMap(SERIALIZED_STONE_MAP);
            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual(PlayerSideSelection.BurnPigeons, _mapController.GetPlayerSideSelection());

            // Game automatically ends because there are no flammable tiles
            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.True(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You lose! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughMapWithNoPigeonsWhileSavingPigeons() {
            GenerateMap(SERIALIZED_MAP_WITH_NO_PIGEONS);
            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());

            // Game automatically ends because there are no pigeons
            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(0, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.True(_mapController.AreAllPigeonsDead());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You lose! No pigeons survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughMapWithNoPigeonsWhileBurningPigeons() {
            GenerateMap(SERIALIZED_MAP_WITH_NO_PIGEONS);
            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual(PlayerSideSelection.BurnPigeons, _mapController.GetPlayerSideSelection());

            // Game automatically ends because there are no pigeons
            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(0, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.True(_mapController.AreAllPigeonsDead());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You win! No pigeons survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughMapWithNoInitialFireWhileSavingPigeons() {
            GenerateMap(SERIALIZED_MAP_WITH_NO_INITIAL_FIRE);
            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            _mapController.EndTurn();

            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            _mapController.EndTurn();

            Assert.AreEqual(4, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            _mapController.EndTurn();

            Assert.AreEqual(3, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            _mapController.EndTurn();

            Assert.AreEqual(2, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            _mapController.EndTurn();

            Assert.AreEqual(1, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.False(_mapController.IsGameOver());
            _mapController.EndTurn();

            // End of game
            Assert.AreEqual(0, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You win! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughMapWithNoInitialFireWhileBurningPigeons() {
            GenerateMap(SERIALIZED_MAP_WITH_NO_INITIAL_FIRE);
            ITurnController turnController = _mapController.GetTurnController();

            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual(PlayerSideSelection.BurnPigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            turnController.SetMoveType(MoveType.Fire);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(1, 1)));
            _mapController.EndTurn();

            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            turnController.SetMoveType(MoveType.Fire);
            Assert.True(turnController.ProcessAction(_mapController.GetTileController(2, 2)));
            _mapController.EndTurn();

            Assert.AreEqual(4, _mapController.GetTurnsLeft());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            // Try to set fire on a tile that was already on fire
            turnController.SetMoveType(MoveType.Fire);
            Assert.False(turnController.ProcessAction(_mapController.GetTileController(1, 2)));
            Assert.False(_mapController.IsGameOver());
            _mapController.EndTurn();

            // Pigeon Dies and Game Ends
            Assert.AreEqual(3, _mapController.GetTurnsLeft());
            Assert.AreEqual(0, _mapController.GetLivePigeonCount());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.True(_mapController.AreAllPigeonsDead());
            Assert.True(_mapController.IsGameOver());
            Assert.AreEqual("You win! No pigeons survived!", _mapController.GetGameOverPlayerStatus());
        }

        private void GenerateMap(string map) {
            _mapController.GenerateMap(map);
            _mapController.SetTurnResolver(_turnResolver);
        }
    }
}