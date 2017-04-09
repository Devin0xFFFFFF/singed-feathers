using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Utility;
using NUnit.Framework;
using NSubstitute;

namespace Assets.Editor.UnitTests.UtilityTests {
    [TestFixture]
    public class MapMakerInputValidatorTest {
        private ITileController _tileController;

        [SetUp]
        public void Init() {
            _tileController = Substitute.For<ITileController>();
            _tileController.IsFlammable().Returns(true);
        }

        [Test]
        public void TestSuccessIfValidInput() {
            MapMakerValidationResult result = MapMakerInputValidator.ValidateInput("some map", "some person",
                new List<Position>() { new Position(0, 0) },
                new List<ITileController>() { _tileController });
            Assert.AreEqual(MapMakerValidationResult.Valid, result);
        }

        [Test]
        public void TestFailureIfNullPigeons() {
            MapMakerValidationResult result = MapMakerInputValidator.ValidateInput("some map", "some person", null,
                new List<ITileController>() { _tileController });
            Assert.AreEqual(MapMakerValidationResult.InvalidNoPigeons, result);
        }

        [Test]
        public void TestFailureIfEmptyPigeons() {
            MapMakerValidationResult result = MapMakerInputValidator.ValidateInput("some map", "some person",
                new List<Position>(), new List<ITileController>() { _tileController });
            Assert.AreEqual(MapMakerValidationResult.InvalidNoPigeons, result);
        }

        [Test]
        public void TestFailureIfNullTiles() {
            MapMakerValidationResult result = MapMakerInputValidator.ValidateInput("some map", "some person",
                new List<Position>() { new Position(0, 0) }, null);
            Assert.AreEqual(MapMakerValidationResult.InvalidNoFlammableTiles, result);
        }

        [Test]
        public void TestFailureIfEmptyTiles() {
            MapMakerValidationResult result = MapMakerInputValidator.ValidateInput("some map", "some person",
                new List<Position>() { new Position(0, 0) }, new List<ITileController>());
            Assert.AreEqual(MapMakerValidationResult.InvalidNoFlammableTiles, result);
        }

        [Test]
        public void TestFailureIfNoFlammableTiles() {
            _tileController.IsFlammable().Returns(false);

            MapMakerValidationResult result = MapMakerInputValidator.ValidateInput("some map", "some person",
                new List<Position>() { new Position(0, 0) }, new List<ITileController>() { _tileController });
            Assert.AreEqual(MapMakerValidationResult.InvalidNoFlammableTiles, result);
        }

        [Test]
        public void TestFailureIfMapNameIsNullOrEmptyOrTooLong() {
            MapMakerValidationResult result = MapMakerInputValidator.ValidateInput(null, "some person",
                new List<Position>() { new Position(0, 0) }, new List<ITileController>() { _tileController });
            Assert.AreEqual(MapMakerValidationResult.InvalidInput, result);

            result = MapMakerInputValidator.ValidateInput("", "some person",
                new List<Position>() { new Position(0, 0) }, new List<ITileController>() { _tileController });
            Assert.AreEqual(MapMakerValidationResult.InvalidInput, result);

            result = MapMakerInputValidator.ValidateInput("the greatest and best map you ever did see in your entire life",
                "some author", new List<Position>() { new Position(0, 0) }, new List<ITileController>() { _tileController });
            Assert.AreEqual(MapMakerValidationResult.InvalidInput, result);
        }

        [Test]
        public void TestFailureIfAuthorNameIsNullOrEmptyOrTooLong() {
            MapMakerValidationResult result = MapMakerInputValidator.ValidateInput("some map", null,
                new List<Position>() { new Position(0, 0) }, new List<ITileController>() { _tileController });
            Assert.AreEqual(MapMakerValidationResult.InvalidInput, result);

            result = MapMakerInputValidator.ValidateInput("some map", "",
                new List<Position>() { new Position(0, 0) }, new List<ITileController>() { _tileController });
            Assert.AreEqual(MapMakerValidationResult.InvalidInput, result);

            result = MapMakerInputValidator.ValidateInput("the greatest and best map you ever did see in your entire life",
                "some author", new List<Position>() { new Position(0, 0) }, new List<ITileController>() { _tileController });
            Assert.AreEqual(MapMakerValidationResult.InvalidInput, result);
        }
    }
}