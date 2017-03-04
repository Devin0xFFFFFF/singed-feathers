using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using NSubstitute;
using NUnit.Framework;

namespace Assets.Editor.ControllerTests {
    [TestFixture]
    public class PigeonControllerTest {
        private PigeonController _pigeonController;
        private ITileController _tileController;
        private ITileController _neighbourTile0;
        private ITileController _neighbourTile1;

        [SetUp]
        public void Init() {
            _tileController = Substitute.For<ITileController>();
            _tileController.Position.Returns(new Position(1, 0));

            _neighbourTile0 = Substitute.For<ITileController>();
            _neighbourTile0.Position.Returns(new Position(0, 1));

            _neighbourTile1 = Substitute.For<ITileController>();
            _neighbourTile1.Position.Returns(new Position(0, 2));

            _tileController.GetNeighbours().Returns(new List<ITileController>() { _neighbourTile0, _neighbourTile1 } );
            _pigeonController = new PigeonController(_tileController);
        }

        [Test]
        public void TestInitialization() {
            Assert.False(_pigeonController.IsDead());
            Assert.AreEqual(100, _pigeonController.GetHealth());
        }

        [Test]
        public void TestKillingPigeon() {
            Assert.False(_pigeonController.IsDead());
            Assert.True(_pigeonController.Kill());
            Assert.True(_pigeonController.IsDead());
            Assert.AreEqual(0, _pigeonController.GetHealth());
        }

        [Test]
        public void TestPigeonFlipsOccupiedFlagWhenArrivingAndLeaving() {
            // Set current tile to be on fire, and neighbourTile0 to be safe and available
            _tileController.IsOnFire().Returns(true);
            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile0.CanBeOccupied().Returns(true);
            
            Assert.False(_pigeonController.HasMoved());
            Assert.True(_pigeonController.Move());
            Assert.True(_pigeonController.HasMoved());
            _tileController.Received().LeaveTile();
            _neighbourTile0.Received().OccupyTile();
            Assert.AreEqual(_pigeonController.CurrentPosition, _neighbourTile0.Position);
        }

        [Test]
        public void TestPigeonOnlyMovesIfCurrentTileOnFire() {
            // Set up safe, available neighbour
            _neighbourTile0.CanBeOccupied().Returns(true);
            _neighbourTile0.IsOnFire().Returns(false);

            // Current tile is not on fire; doesn't move
            _tileController.IsOnFire().Returns(false);

            Assert.False(_pigeonController.Move());
            Assert.False(_pigeonController.HasMoved());

            // Current tile is on fire; moves to safe neighbour
            _tileController.IsOnFire().Returns(true);

            Assert.False(_pigeonController.HasMoved());
            Assert.True(_pigeonController.Move());
            Assert.True(_pigeonController.HasMoved());
            _tileController.Received().LeaveTile();
            _neighbourTile0.Received().OccupyTile();
            Assert.AreEqual(_pigeonController.CurrentPosition, _neighbourTile0.Position);
        }

        [Test]
        public void TestMoveToSafeTile() {
            // Set up unsafe neighbour
            _neighbourTile0.CanBeOccupied().Returns(true);
            _neighbourTile0.IsOnFire().Returns(true);

            // Set up safe neighbour
            _neighbourTile1.CanBeOccupied().Returns(true);
            _neighbourTile1.IsOnFire().Returns(false);

            _tileController.IsOnFire().Returns(true);
            Assert.False(_pigeonController.HasMoved());
            Assert.True(_pigeonController.Move());
            Assert.True(_pigeonController.HasMoved());

            // Should have moved to neighbourTile1
            Assert.AreEqual(_pigeonController.CurrentPosition, _neighbourTile1.Position);
        }

        [Test] public void TestStaysInPlaceIfNoSafeTile() {
            _tileController.IsOnFire().Returns(true);

            // All tiles in vicinity are on fire, but are available to be moved to
            _neighbourTile0.IsOnFire().Returns(true);
            _neighbourTile0.CanBeOccupied().Returns(true);
            _neighbourTile1.IsOnFire().Returns(true);
            _neighbourTile1.CanBeOccupied().Returns(true);

            Assert.False(_pigeonController.HasMoved());
            Assert.False(_pigeonController.Move());
            Assert.False(_pigeonController.HasMoved());
            _tileController.DidNotReceive().LeaveTile();
        }

        [Test]
        public void TestMovesToUnoccupiedTile() {
            _tileController.IsOnFire().Returns(true);

            // All tiles in vicinity are in safe, but only one is available to be moved to
            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile0.CanBeOccupied().Returns(false);
            _neighbourTile1.IsOnFire().Returns(false);
            _neighbourTile1.CanBeOccupied().Returns(false);

            Assert.False(_pigeonController.HasMoved());
            Assert.False(_pigeonController.Move());
            Assert.False(_pigeonController.HasMoved());
            _tileController.DidNotReceive().LeaveTile();
            _neighbourTile0.DidNotReceive().OccupyTile();
            _neighbourTile1.DidNotReceive().OccupyTile();
        }

        [Test]
        public void TestStaysInPlaceIfNoUnoccupiedTile() {
            _tileController.IsOnFire().Returns(true);

            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile0.CanBeOccupied().Returns(false);
            _neighbourTile1.IsOnFire().Returns(false);
            _neighbourTile1.CanBeOccupied().Returns(false);

            Assert.False(_pigeonController.HasMoved());
            Assert.False(_pigeonController.Move());
            Assert.False(_pigeonController.HasMoved());
        }

        [Test] public void TestPigeonTakesNoDamageWhenCurrentTileIsNotOnFireAndNoNeighbourIsOnFire() {
            _tileController.IsOnFire().Returns(false);
            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile1.IsOnFire().Returns(false);

            Assert.AreEqual(100, _pigeonController.GetHealth());
            _pigeonController.UpdateHealth();
            
            // All relevant tiles were queried for onfire status
            _tileController.Received().IsOnFire();
            _neighbourTile0.Received().IsOnFire();
            _neighbourTile1.Received().IsOnFire();
            
            Assert.AreEqual(100, _pigeonController.GetHealth());
        }

        [Test]
        public void TestPigeonTakesDamageWhenCurrentTileIsOnFireAndNoNeighbourIsOnFire() {
            _tileController.IsOnFire().Returns(true);
            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile1.IsOnFire().Returns(false);

            Assert.AreEqual(100, _pigeonController.GetHealth());
            _pigeonController.UpdateHealth();

            // All relevant tiles were queried for onfire status
            _tileController.Received().IsOnFire();
            _neighbourTile0.Received().IsOnFire();
            _neighbourTile1.Received().IsOnFire();

            Assert.AreEqual(80, _pigeonController.GetHealth());
        }

        [Test]
        public void TestPigeonTakesDamageWhenCurrentTileIsNotOnFireAndOneNeighbourIsOnFire() {
            _tileController.IsOnFire().Returns(false);
            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile1.IsOnFire().Returns(true);

            Assert.AreEqual(100, _pigeonController.GetHealth());
            _pigeonController.UpdateHealth();

            // All relevant tiles were queried for onfire status
            _tileController.Received().IsOnFire();
            _neighbourTile0.Received().IsOnFire();
            _neighbourTile1.Received().IsOnFire();

            Assert.AreEqual(90, _pigeonController.GetHealth());
        }

        [Test]
        public void TestPigeonTakesDamageWhenCurrentTileIsOnFireAndOneNeighbourIsOnFire() {
            _tileController.IsOnFire().Returns(true);
            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile1.IsOnFire().Returns(true);

            Assert.AreEqual(100, _pigeonController.GetHealth());
            _pigeonController.UpdateHealth();

            // All relevant tiles were queried for onfire status
            _tileController.Received().IsOnFire();
            _neighbourTile0.Received().IsOnFire();
            _neighbourTile1.Received().IsOnFire();

            Assert.AreEqual(70, _pigeonController.GetHealth());
        }
    }
}