using NUnit.Framework;
using System.Collections.Generic;
using NSubstitute;
using CoreGame.Models;
using CoreGame.Controllers.Interfaces;
using CoreGame.Utility;

namespace Assets.Editor.UnitTests.UtilityTests {
    [TestFixture]
    public class TurnResolveUtilityTest {
        private Map _map;
        private IPigeonController _pigeon0;
        private IPigeonController _pigeon1;
        private ITileController _tile0;
        private ITileController _tile1;
        private ITileController _tile2;
        private ITileController _tile3;

        [SetUp]
        public void Init() { _map = GenerateTestMap(); }

        [Test]
        public void TestAllPigeonsCallReactEvenIfDead() {
            _pigeon0.IsDead().Returns(true);
            _pigeon0.Move().Returns(true); // If invoked, return true
            _pigeon1.IsDead().Returns(false);
            _pigeon1.Move().Returns(true); // If invoked, return true

            TurnResolveUtility.MovePigeons(_map);

            // Pigeon0 React() invoked
            _pigeon0.Received().React();

            // Pigeon1 React() invoked
            _pigeon1.Received().React();
        }

        [Test]
        public void TestAllTilesSpreadFireAndUpKeep() {
            TurnResolveUtility.SpreadFires(_map);
            Received.InOrder(() => {
                _tile0.SpreadFire();
                _tile1.SpreadFire();
                _tile2.SpreadFire();
                _tile3.SpreadFire();

                _tile0.UpKeep();
                _tile1.UpKeep();
                _tile2.UpKeep();
                _tile3.UpKeep();
            });
        }

        private Map GenerateTestMap() {
            return new Map() {
                Height = 2,
                Width = 2,
                InitialFirePositions = null,
                InitialPigeonPositions = null,
                TileMap = IntializeControllers(),
                Pigeons = InitializePigeons(),
                TurnController = null,
                TurnResolver = null
            };
        }

        private IList<IPigeonController> InitializePigeons() {
            _pigeon0 = Substitute.For<IPigeonController>();
            _pigeon1 = Substitute.For<IPigeonController>();

            IList<IPigeonController> pigeons = new List<IPigeonController>() { _pigeon0, _pigeon1 };
            return pigeons;
        }

        private ITileController[,] IntializeControllers() {
            _tile0 = Substitute.For<ITileController>();
            _tile1 = Substitute.For<ITileController>();
            _tile2 = Substitute.For<ITileController>();
            _tile3 = Substitute.For<ITileController>();

            ITileController[,] tiles = { { _tile0, _tile1 }, {_tile2 , _tile3 } };
            return tiles;
        }
    }
}
