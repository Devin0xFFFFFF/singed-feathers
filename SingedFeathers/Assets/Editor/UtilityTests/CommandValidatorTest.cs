using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;
using System;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.Commands;
using CoreGame.Utility;

namespace Assets.Editor.UtilityTests {
    [TestFixture]
    public class CommandValidatorTest {
        private Map _map;
        private const int TEST_HEIGHT = 2;
        private const int TEST_WIDTH = 3;
        private ITileController[,] _tileMap;
        private ICommand _command;

        [SetUp]
        public void Init() {
            _map = GenerateTestMap();
            InitializeTileMap();
            InitializeCommand();
            ConfigurePositionValidator();

            CommandValidator.InitializeValues(_map);
        }

        [Test]
        public void TestDeltaValidationAtValidMapLocationAndTileControllerCanBeExcecutedOn() {
            _command.CanBeExecutedOnTile(Arg.Any<ITileController>()).Returns(true);

            List<Delta> deltas = new List<Delta>() {
                new Delta(new Position(0, 1), _command),
                new Delta(new Position(1, 0), _command)
            };

            Assert.True(CommandValidator.ValidateDeltas(deltas, _tileMap));
        }

        [Test]
        public void TestDeltaValidationAtInalidMapLocationAndTileControllerCanBeExcecutedOn() {
            _command.CanBeExecutedOnTile(Arg.Any<ITileController>()).Returns(true);

            List<Delta> deltas = new List<Delta>() {
                // Only one needs to be invalid for validation to fail
                new Delta(new Position(7, 1), _command),
                new Delta(new Position(1, 0), _command)
            };

            Assert.False(CommandValidator.ValidateDeltas(deltas, _tileMap));
        }

        [Test]
        public void TestDeltaValidationAtValidMapLocationAndTileControllerCannotBeExcecutedOn() {
            _command.CanBeExecutedOnTile(Arg.Any<ITileController>()).Returns(false);

            List<Delta> deltas = new List<Delta>() {
                new Delta(new Position(0, 1), _command),
                new Delta(new Position(1, 0), _command)
            };

            Assert.False(CommandValidator.ValidateDeltas(deltas, _tileMap));
        }

        [Test]
        public void TestDeltaValidationAtInalidMapLocationAndTileControllerCannotBeExcecutedOn() {
            _command.CanBeExecutedOnTile(Arg.Any<ITileController>()).Returns(false);

            List<Delta> deltas = new List<Delta>() {
                new Delta(new Position(0, 1), _command),
                new Delta(new Position(7, 0), _command)
            };

            Assert.False(CommandValidator.ValidateDeltas(deltas, _tileMap));
        }

        [Test]
        public void TestDeltaValidationWithMoreMovesThanAllowed() {
            // Max moves per turn is 2
            List<Delta> deltas = new List<Delta>() {
                new Delta(new Position(1, 0), _command),
                new Delta(new Position(1, 0), _command),
                new Delta(new Position(1, 0), _command)
            };

            Assert.False(CommandValidator.ValidateDeltas(deltas, _tileMap));
        }

        [Test]
        public void TestNullDeltaListThrowsException() {
            try {
                CommandValidator.ValidateDeltas(null, _tileMap);
                Assert.Fail();
            } catch (Exception) {
                Assert.Pass();
            }
        }

        [Test]
        public void TestNullTileMapThrowsException() {
            try {
                CommandValidator.ValidateDeltas(new List<Delta>(), null);
                Assert.Fail();
            } catch (Exception) {
                Assert.Pass();
            }
        }

        [Test]
        public void TestEmptyDeltaListPassesValidation() {
            Assert.True(CommandValidator.ValidateDeltas(new List<Delta>(), _tileMap));
        }

        private Map GenerateTestMap() {
            return new Map() {
                Height = TEST_HEIGHT,
                Width = TEST_WIDTH,
                MaxMovesPerTurn = 2
            };
        }

        private void InitializeTileMap() {
            _tileMap = new ITileController[TEST_WIDTH, TEST_HEIGHT];
            for (int x = 0; x < TEST_WIDTH; x++) {
                for (int y = 0; y < TEST_HEIGHT; y ++) {
                    _tileMap[x, y] = Substitute.For<ITileController>();
                }
            }
        }

        private void ConfigurePositionValidator() { MapLocationValidator.InitializeValues(_map); }
        
        private void InitializeCommand() { _command = Substitute.For<ICommand>(); }
    }
}
