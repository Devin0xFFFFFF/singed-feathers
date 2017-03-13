using CoreGame.Models;
using CoreGame.Utility;
using NUnit.Framework;

namespace Assets.Editor.UtilityTests {
    [TestFixture]
    public class MapLocationValidatorTest {
        Map _map;

        [SetUp]
        public void Init() {
            _map = GenerateTestMap();
            MapLocationValidator.InitializeValues(_map);
        }

        [Test]
        public void TestCoordinatesAreValid() {
            MapLocationValidator.InitializeValues(_map);
            Assert.False(MapLocationValidator.CoordinatesAreValid(0, -1));
            Assert.False(MapLocationValidator.CoordinatesAreValid(-1, -1));
            Assert.False(MapLocationValidator.CoordinatesAreValid(-1, 0));

            Assert.True(MapLocationValidator.CoordinatesAreValid(0, 0));
            Assert.True(MapLocationValidator.CoordinatesAreValid(1, 0));
            Assert.True(MapLocationValidator.CoordinatesAreValid(2, 0));

            Assert.True(MapLocationValidator.CoordinatesAreValid(0, 1));
            Assert.True(MapLocationValidator.CoordinatesAreValid(1, 1));
            Assert.True(MapLocationValidator.CoordinatesAreValid(2, 1));

            Assert.True(MapLocationValidator.CoordinatesAreValid(0, 2));
            Assert.True(MapLocationValidator.CoordinatesAreValid(1, 2));
            Assert.True(MapLocationValidator.CoordinatesAreValid(2, 2));

            Assert.False(MapLocationValidator.CoordinatesAreValid(0, 3));
            Assert.False(MapLocationValidator.CoordinatesAreValid(3, 3));
            Assert.False(MapLocationValidator.CoordinatesAreValid(3, 0));
        }

        [Test]
        public void TestPositionIsValid() {
            MapLocationValidator.InitializeValues(_map);
            Assert.False(MapLocationValidator.PositionIsValid(new Position(0, -1)));
            Assert.False(MapLocationValidator.PositionIsValid(new Position(-1, -1)));
            Assert.False(MapLocationValidator.PositionIsValid(new Position(-1, 0)));

            Assert.True(MapLocationValidator.PositionIsValid(new Position(0, 0)));
            Assert.True(MapLocationValidator.PositionIsValid(new Position(1, 0)));
            Assert.True(MapLocationValidator.PositionIsValid(new Position(2, 0)));

            Assert.True(MapLocationValidator.PositionIsValid(new Position(0, 1)));
            Assert.True(MapLocationValidator.PositionIsValid(new Position(1, 1)));
            Assert.True(MapLocationValidator.PositionIsValid(new Position(2, 1)));

            Assert.True(MapLocationValidator.PositionIsValid(new Position(0, 2)));
            Assert.True(MapLocationValidator.PositionIsValid(new Position(1, 2)));
            Assert.True(MapLocationValidator.PositionIsValid(new Position(2, 2)));

            Assert.False(MapLocationValidator.PositionIsValid(new Position(0, 3)));
            Assert.False(MapLocationValidator.PositionIsValid(new Position(3, 3)));
            Assert.False(MapLocationValidator.PositionIsValid(new Position(3, 0)));
        }

        private Map GenerateTestMap() {
            return new Map() {
                Height = 3,
                Width = 3,
                InitialFirePosition = null,
                InitialPigeonPositions = null,
                TileMap = null,
                Pigeons = null,
                TurnController = null,
                TurnResolver = null
            };
        }
    }
}