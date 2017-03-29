using NUnit.Framework;
using CoreGame.Controllers.Interfaces;
using CoreGame.Controllers;
using CoreGame.Models;

namespace Assets.Editor.IntegrationTests {
    [TestFixture]
    public class MapTest {
        private const string SERIALIZED_MAP = "{'TurnsLeft': 10, 'MaxMovesPerTurn': 1, 'Width': 8, 'Height': 8, 'InitialFirePositions': [{'X': 2,'Y': 3}, {'X': 6,'Y': 1}], 'InitialPigeonPositions': [{'X': 3, 'Y': 1}, {'X': 6, 'Y': 6}], 'RawMap': [[3, 2, 2, 2, 2, 1, 3, 1], [3, 2, 1, 2, 2, 3, 3, 2], [2, 2, 2, 2, 2, 2, 2, 1], [1, 2, 2, 1, 2, 1, 3, 2], [1, 2, 2, 2, 1, 1, 3, 2], [1, 2, 2, 2, 2, 1, 3, 1], [3, 2, 1, 2, 2, 3, 3, 2], [3, 2, 1, 2, 1, 2, 2, 1]]}";
        private IMapController _mapController;
        private ITurnResolver _turnResolver;

        [SetUp]
        public void Init() {
            _mapController = new MapController(); 
            _turnResolver = new LocalTurnResolver();
        }

        [Test]
        public void TestMapGeneratesProperly() {
            GenerateMap();

            // Test Map was generated properly
            Assert.AreEqual(8, _mapController.Width);
            Assert.AreEqual(8, _mapController.Height);
            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());
            Assert.AreEqual("You won! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
        }

        [Test]
        public void PlayThroughSavingPigeonsJustHittingEndTurn() {
            GenerateMap();
            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(4, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(3, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(2, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(1, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(0, _mapController.GetTurnsLeft());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            Assert.AreEqual("You won! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }
            
        [Test]
        public void PlayThroughBurningPigeonsJustHittingEndTurn() {
            GenerateMap();
            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual(PlayerSideSelection.BurnPigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(4, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(3, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(2, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(1, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(0, _mapController.GetTurnsLeft());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.AreEqual(2, _mapController.GetLivePigeonCount());
            Assert.AreEqual("You lost! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughKillingOnePigeonWhileTryingToSavePigeons() {
            GenerateMap();
            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            _mapController.ApplyHeat(4, 1);
            _mapController.EndTurn();
            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            // First Pigeon Dies
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(4, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(3, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(2, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(1, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(0, _mapController.GetTurnsLeft());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.AreEqual("You won! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughKillingOnePigeonWhileTryingToBurnPigeons() {
            GenerateMap();
            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual(PlayerSideSelection.BurnPigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            _mapController.ApplyHeat(4, 1);
            _mapController.EndTurn();
            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            // First Pigeon Dies
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(4, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(3, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(2, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(1, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            Assert.AreEqual(0, _mapController.GetTurnsLeft());
            Assert.False(_mapController.IsMapBurntOut());
            Assert.False(_mapController.AreAllPigeonsDead());
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            Assert.AreEqual("You lost! A pigeon survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughKillingAllPigeonsWhileTryingToSavePigeons() {
            GenerateMap();
            _mapController.SetPlayerSideSelection(PlayerSideSelection.SavePigeons);
            Assert.AreEqual(PlayerSideSelection.SavePigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            _mapController.ApplyHeat(4, 1);
            _mapController.EndTurn();
            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            _mapController.ApplyHeat(6, 7);
            _mapController.EndTurn();
            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            _mapController.ApplyHeat(6, 6);
            _mapController.EndTurn();
            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            _mapController.ApplyHeat(6, 3);
            _mapController.EndTurn();
            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            // First Pigeon Dies
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            _mapController.ApplyHeat(6, 4);
            _mapController.EndTurn();
            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            // Second Pigeon Dies and Game Ends
            Assert.False(_mapController.IsMapBurntOut());
            Assert.True(_mapController.AreAllPigeonsDead());
            Assert.AreEqual(0, _mapController.GetLivePigeonCount());
            Assert.AreEqual("You lost! No pigeons survived!", _mapController.GetGameOverPlayerStatus());
        }

        [Test]
        public void PlayThroughKillingAllPigeonsWhileTryingToBurnPigeons() {
            GenerateMap();
            _mapController.SetPlayerSideSelection(PlayerSideSelection.BurnPigeons);
            Assert.AreEqual(PlayerSideSelection.BurnPigeons, _mapController.GetPlayerSideSelection());

            Assert.AreEqual(10, _mapController.GetTurnsLeft());
            _mapController.ApplyHeat(4, 1);
            _mapController.EndTurn();
            Assert.AreEqual(9, _mapController.GetTurnsLeft());
            _mapController.ApplyHeat(6, 7);
            _mapController.EndTurn();
            Assert.AreEqual(8, _mapController.GetTurnsLeft());
            _mapController.ApplyHeat(6, 6);
            _mapController.EndTurn();
            Assert.AreEqual(7, _mapController.GetTurnsLeft());
            _mapController.ApplyHeat(6, 3);
            _mapController.EndTurn();
            Assert.AreEqual(6, _mapController.GetTurnsLeft());
            // First Pigeon Dies
            Assert.AreEqual(1, _mapController.GetLivePigeonCount());
            _mapController.ApplyHeat(6, 4);
            _mapController.EndTurn();
            Assert.AreEqual(5, _mapController.GetTurnsLeft());
            _mapController.EndTurn();
            // Second Pigeon Dies and Game Ends
            Assert.False(_mapController.IsMapBurntOut());
            Assert.True(_mapController.AreAllPigeonsDead());
            Assert.AreEqual(0, _mapController.GetLivePigeonCount());
            Assert.AreEqual("You won! No pigeons survived!", _mapController.GetGameOverPlayerStatus());
        }

        private void GenerateMap() {
            _mapController.GenerateMap(SERIALIZED_MAP);
            _mapController.SetTurnResolver(_turnResolver);
        }
    }
}