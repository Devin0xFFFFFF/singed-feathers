using NUnit.Framework;
using NSubstitute;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.Commands;
using CoreGame.Utility;

namespace Assets.Editor.UnitTests.UtilityTests {
    [TestFixture]
    public class CommandValidatorTest {
        private Map _map;
        private const int TEST_HEIGHT = 2;
        private const int TEST_WIDTH = 3;
        private ITileController[,] _tileMap;
        private Command _command;

        [SetUp]
        public void Init() {
            _map = GenerateTestMap();
            InitializeTileMap();
            InitializeCommand();
            ConfigurePositionValidator();
        }

        [Test]
        public void TestDeltaValidationAtValidMapLocationAndTileControllerCanBeExcecutedOn() {
            Assert.True(CommandValidator.ValidateDelta(new Delta(new Position(0, 1), _command), _tileMap));
            Assert.True(CommandValidator.ValidateDelta(new Delta(new Position(1, 0), _command), _tileMap));
        }

        [Test]
        public void TestDeltaValidationAtInalidMapLocationAndTileControllerCanBeExcecutedOn() {
            Assert.False(CommandValidator.ValidateDelta(new Delta(new Position(7, 1), _command), _tileMap));
            Assert.True(CommandValidator.ValidateDelta(new Delta(new Position(1, 0), _command), _tileMap));
        }

        [Test]
        public void TestDeltaValidationAtValidMapLocationAndTileControllerCannotBeExcecutedOn() {
            _tileMap[0, 1].IsFlammable().Returns(false);
            _tileMap[1, 0].IsFlammable().Returns(false);

            Assert.False(CommandValidator.ValidateDelta(new Delta(new Position(0, 1), _command), _tileMap));
            Assert.False(CommandValidator.ValidateDelta(new Delta(new Position(1, 0), _command), _tileMap));
        }

        [Test]
        public void TestDeltaValidationAtInalidMapLocationAndTileControllerCannotBeExcecutedOn() {
            _tileMap[1, 0].IsFlammable().Returns(false);

            Assert.False(CommandValidator.ValidateDelta(new Delta(new Position(7, 1), _command), _tileMap));
            Assert.False(CommandValidator.ValidateDelta(new Delta(new Position(1, 0), _command), _tileMap));
        }

        [Test]
        public void TestNullTileMapThrowsException() {
            Assert.False(CommandValidator.ValidateDelta(new Delta(new Position(0, 1), _command), null));
        }

        [Test]
        public void TestNullDeltaListPassesValidation() {
            Assert.True(CommandValidator.ValidateDelta(null, _tileMap));
        }

        private Map GenerateTestMap() {
            return new Map() {
                Height = TEST_HEIGHT,
                Width = TEST_WIDTH
            };
        }

        private void InitializeTileMap() {
            _tileMap = new ITileController[TEST_WIDTH, TEST_HEIGHT];
            for (int x = 0; x < TEST_WIDTH; x++) {
                for (int y = 0; y < TEST_HEIGHT; y ++) {
                    _tileMap[x, y] = Substitute.For<ITileController>();
                    _tileMap[x, y].IsFlammable().Returns(true);
                }
            }
        }

        private void ConfigurePositionValidator() { MapLocationValidator.InitializeValues(_map); }
        
        private void InitializeCommand() { _command = new Command(MoveType.Water); }
    }
}