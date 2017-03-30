using System.Collections.Generic;
using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using NSubstitute;
using NUnit.Framework;

namespace Assets.Editor.UnitTests.ControllerTests {
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
            Assert.AreEqual(50, _pigeonController.GetHealth());
        }

        [Test]
        public void TestKillingPigeon() {
            Assert.False(_pigeonController.IsDead());
            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());

            Assert.True(_pigeonController.Kill());

            Assert.True(_pigeonController.IsDead());
            Assert.AreEqual(0, _pigeonController.GetHealth());
        }

        [Test]
        public void TestKillingPigeonReturnsTrueIfNewlyDeadAndFalseIfAlreadyDead() {
            Assert.False(_pigeonController.IsDead());
            Assert.True(_pigeonController.Kill());
            Assert.True(_pigeonController.IsDead());

            // Killing dead pigeon returns false
            Assert.False(_pigeonController.Kill());
            Assert.True(_pigeonController.IsDead());
        }

        [Test]
        public void TestPigeonFlipsOccupiedFlagWhenArrivingAndLeaving() {
            // Set current tile to be on fire, and neighbourTile0 to be safe and available
            _tileController.IsOnFire().Returns(true);
            _tileController.GetTileHeat().Returns(100);
            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile0.GetTileHeat().Returns(0);
            _neighbourTile0.CanBeOccupied().Returns(true);
            
            Assert.False(_pigeonController.HasMoved());
            Assert.True(_pigeonController.Move());
            Assert.True(_pigeonController.HasMoved());
            _tileController.Received().MarkUnoccupied();
            _neighbourTile0.Received().MarkOccupied();
            Assert.AreEqual(_pigeonController.CurrentPosition, _neighbourTile0.Position);
        }

        [Test]
        public void TestPigeonMovesIfCurrentTileOnFire() {
            // Set up safe, available neighbour
            _neighbourTile0.CanBeOccupied().Returns(true);
            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile0.GetTileHeat().Returns(0);

            // Current tile is not on fire; doesn't move
            _tileController.IsOnFire().Returns(false);
            _tileController.GetTileHeat().Returns(0);

            Assert.False(_pigeonController.Move());
            Assert.False(_pigeonController.HasMoved());

            // Current tile is on fire; moves to safe neighbour
            _tileController.IsOnFire().Returns(true);
            _tileController.GetTileHeat().Returns(100);

            Assert.False(_pigeonController.HasMoved());
            Assert.True(_pigeonController.Move());
            Assert.True(_pigeonController.HasMoved());
            _tileController.Received().MarkUnoccupied();
            _neighbourTile0.Received().MarkOccupied();
            Assert.AreEqual(_pigeonController.CurrentPosition, _neighbourTile0.Position);
        }

        [Test]
        public void TestMoveToSafeTile() {
            // Set up unsafe neighbour
            _neighbourTile0.CanBeOccupied().Returns(true);
            _neighbourTile0.IsOnFire().Returns(true);
            _neighbourTile0.GetTileHeat().Returns(100);

            // Set up safe neighbour
            _neighbourTile1.CanBeOccupied().Returns(true);
            _neighbourTile1.IsOnFire().Returns(false);
            _neighbourTile0.GetTileHeat().Returns(0);

            _tileController.IsOnFire().Returns(true);
            _tileController.GetTileHeat().Returns(100);
            Assert.False(_pigeonController.HasMoved());
            Assert.True(_pigeonController.Move());
            Assert.True(_pigeonController.HasMoved());

            // Should have moved to neighbourTile1
            Assert.AreEqual(_pigeonController.CurrentPosition, _neighbourTile1.Position);
        }

        [Test]
        public void TestMoveAwayFromHottestTile() {
            ITileController tileController = Substitute.For<ITileController>();
            tileController.Position.Returns(new Position(1, 1));
            tileController.CanBeOccupied().Returns(true);
            tileController.IsOnFire().Returns(false);
            tileController.GetTileHeat().Returns(0);

            ITileController neighbourTile0 = Substitute.For<ITileController>();
            neighbourTile0.Position.Returns(new Position(0, 1));
            neighbourTile0.CanBeOccupied().Returns(true);
            neighbourTile0.IsOnFire().Returns(true);
            neighbourTile0.GetTileHeat().Returns(100000);

            ITileController neighbourTile1 = Substitute.For<ITileController>();
            neighbourTile1.Position.Returns(new Position(1, 0));
            neighbourTile1.CanBeOccupied().Returns(true);
            neighbourTile1.IsOnFire().Returns(false);
            neighbourTile1.GetTileHeat().Returns(0);

            ITileController neighbourTile2 = Substitute.For<ITileController>();
            neighbourTile2.Position.Returns(new Position(1, 2));
            neighbourTile2.CanBeOccupied().Returns(true);
            neighbourTile2.IsOnFire().Returns(false);
            neighbourTile2.GetTileHeat().Returns(0);

            ITileController neighbourTile3 = Substitute.For<ITileController>();
            neighbourTile3.Position.Returns(new Position(2, 1));
            neighbourTile3.CanBeOccupied().Returns(true);
            neighbourTile3.IsOnFire().Returns(false);
            neighbourTile3.GetTileHeat().Returns(0);

            tileController.GetNeighbours().Returns(new List<ITileController>() { neighbourTile0, neighbourTile1, neighbourTile2, neighbourTile3 });
            _pigeonController = new PigeonController(tileController);

            Assert.False(_pigeonController.HasMoved());
            Assert.True(_pigeonController.Move());
            Assert.True(_pigeonController.HasMoved());

            // Should have moved to neighbourTile1
            Assert.AreEqual(_pigeonController.CurrentPosition, neighbourTile3.Position);
        }

        [Test]
        public void TestMoveAwayFromHottestTileIfUnpassible() {
            ITileController tileController = Substitute.For<ITileController>();
            tileController.Position.Returns(new Position(1, 1));
            tileController.CanBeOccupied().Returns(true);
            tileController.IsOnFire().Returns(false);
            tileController.GetTileHeat().Returns(0);

            ITileController neighbourTile0 = Substitute.For<ITileController>();
            neighbourTile0.Position.Returns(new Position(0, 1));
            neighbourTile0.CanBeOccupied().Returns(true);
            neighbourTile0.IsOnFire().Returns(true);
            neighbourTile0.GetTileHeat().Returns(100000);

            ITileController neighbourTile1 = Substitute.For<ITileController>();
            neighbourTile1.Position.Returns(new Position(1, 0));
            neighbourTile1.CanBeOccupied().Returns(true);
            neighbourTile1.IsOnFire().Returns(false);
            neighbourTile1.GetTileHeat().Returns(0);

            ITileController neighbourTile2 = Substitute.For<ITileController>();
            neighbourTile2.Position.Returns(new Position(1, 2));
            neighbourTile2.CanBeOccupied().Returns(true);
            neighbourTile2.IsOnFire().Returns(false);
            neighbourTile2.GetTileHeat().Returns(0);

            ITileController neighbourTile3 = Substitute.For<ITileController>();
            neighbourTile3.Position.Returns(new Position(2, 1));
            neighbourTile3.CanBeOccupied().Returns(false);
            neighbourTile3.IsOnFire().Returns(false);
            neighbourTile3.GetTileHeat().Returns(0);

            tileController.GetNeighbours().Returns(new List<ITileController>() { neighbourTile0, neighbourTile1, neighbourTile2, neighbourTile3 });
            _pigeonController = new PigeonController(tileController);

            Assert.False(_pigeonController.HasMoved());
            Assert.True(_pigeonController.Move());
            Assert.True(_pigeonController.HasMoved());

            // Should have moved to neighbourTile1
            Assert.AreEqual(_pigeonController.CurrentPosition, neighbourTile1.Position);
        }

        [Test]
        public void TestMoveTowardsCoolestTile() {
            ITileController tileController = Substitute.For<ITileController>();
            tileController.Position.Returns(new Position(1, 1));
            tileController.CanBeOccupied().Returns(true);
            tileController.IsOnFire().Returns(false);
            tileController.GetTileHeat().Returns(2);

            ITileController neighbourTile0 = Substitute.For<ITileController>();
            neighbourTile0.Position.Returns(new Position(0, 1));
            neighbourTile0.CanBeOccupied().Returns(true);
            neighbourTile0.IsOnFire().Returns(false);
            neighbourTile0.GetTileHeat().Returns(2);

            ITileController neighbourTile1 = Substitute.For<ITileController>();
            neighbourTile1.Position.Returns(new Position(1, 0));
            neighbourTile1.CanBeOccupied().Returns(true);
            neighbourTile1.IsOnFire().Returns(false);
            neighbourTile1.GetTileHeat().Returns(2);

            ITileController neighbourTile2 = Substitute.For<ITileController>();
            neighbourTile2.Position.Returns(new Position(1, 2));
            neighbourTile2.CanBeOccupied().Returns(true);
            neighbourTile2.IsOnFire().Returns(false);
            neighbourTile2.GetTileHeat().Returns(1);

            ITileController neighbourTile3 = Substitute.For<ITileController>();
            neighbourTile3.Position.Returns(new Position(2, 1));
            neighbourTile3.CanBeOccupied().Returns(true);
            neighbourTile3.IsOnFire().Returns(false);
            neighbourTile3.GetTileHeat().Returns(2);

            tileController.GetNeighbours().Returns(new List<ITileController>() { neighbourTile0, neighbourTile1, neighbourTile2, neighbourTile3 });
            _pigeonController = new PigeonController(tileController);

            Assert.False(_pigeonController.HasMoved());
            Assert.True(_pigeonController.Move());
            Assert.True(_pigeonController.HasMoved());

            // Should have moved to neighbourTile1
            Assert.AreEqual(_pigeonController.CurrentPosition, neighbourTile2.Position);
        }

        [Test]
        public void TestStayStillIfNoHeat() {
            ITileController tileController = Substitute.For<ITileController>();
            tileController.Position.Returns(new Position(1, 1));
            tileController.CanBeOccupied().Returns(true);
            tileController.IsOnFire().Returns(false);
            tileController.GetTileHeat().Returns(0);

            ITileController neighbourTile0 = Substitute.For<ITileController>();
            neighbourTile0.Position.Returns(new Position(0, 1));
            neighbourTile0.CanBeOccupied().Returns(true);
            neighbourTile0.IsOnFire().Returns(false);
            neighbourTile0.GetTileHeat().Returns(0);

            ITileController neighbourTile1 = Substitute.For<ITileController>();
            neighbourTile1.Position.Returns(new Position(1, 0));
            neighbourTile1.CanBeOccupied().Returns(true);
            neighbourTile1.IsOnFire().Returns(false);
            neighbourTile1.GetTileHeat().Returns(0);

            ITileController neighbourTile2 = Substitute.For<ITileController>();
            neighbourTile2.Position.Returns(new Position(1, 2));
            neighbourTile2.CanBeOccupied().Returns(true);
            neighbourTile2.IsOnFire().Returns(false);
            neighbourTile2.GetTileHeat().Returns(0);

            ITileController neighbourTile3 = Substitute.For<ITileController>();
            neighbourTile3.Position.Returns(new Position(2, 1));
            neighbourTile3.CanBeOccupied().Returns(true);
            neighbourTile3.IsOnFire().Returns(false);
            neighbourTile3.GetTileHeat().Returns(0);

            tileController.GetNeighbours().Returns(new List<ITileController>() { neighbourTile0, neighbourTile1, neighbourTile2, neighbourTile3 });
            _pigeonController = new PigeonController(tileController);

            Assert.False(_pigeonController.HasMoved());
            Assert.False(_pigeonController.Move());
            Assert.False(_pigeonController.HasMoved());

            // Should have moved to neighbourTile1
            Assert.AreEqual(_pigeonController.CurrentPosition, tileController.Position);
        }

        [Test]
        public void TestStaysInPlaceIfNoSafeTile() {
            _tileController.IsOnFire().Returns(true);

            // All tiles in vicinity are on fire, but are available to be moved to
            _neighbourTile0.IsOnFire().Returns(true);
            _neighbourTile0.CanBeOccupied().Returns(true);
            _neighbourTile1.IsOnFire().Returns(true);
            _neighbourTile1.CanBeOccupied().Returns(true);

            Assert.False(_pigeonController.HasMoved());
            Assert.False(_pigeonController.Move());
            Assert.False(_pigeonController.HasMoved());
            _tileController.DidNotReceive().MarkUnoccupied();
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
            _tileController.DidNotReceive().MarkUnoccupied();
            _neighbourTile0.DidNotReceive().MarkOccupied();
            _neighbourTile1.DidNotReceive().MarkOccupied();
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

        [Test]
        public void TestPigeonTakesNoDamageWhenCurrentTileIsNotOnFireAndNoNeighbourIsOnFire() {
            _tileController.IsOnFire().Returns(false);
            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile1.IsOnFire().Returns(false);

            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());
            _pigeonController.TakeFireDamage();
            
            // All relevant tiles were queried for onfire status
            _tileController.Received().IsOnFire();
            _neighbourTile0.Received().IsOnFire();
            _neighbourTile1.Received().IsOnFire();
            
            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());
        }

        [Test]
        public void TestPigeonTakesDamageWhenCurrentTileIsOnFireAndNoNeighbourIsOnFire() {
            _tileController.IsOnFire().Returns(true);
            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile1.IsOnFire().Returns(false);

            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());
            _pigeonController.TakeFireDamage();

            // All relevant tiles were queried for onfire status
            _tileController.Received().IsOnFire();
            _neighbourTile0.Received().IsOnFire();
            _neighbourTile1.Received().IsOnFire();

            Assert.AreEqual(Pigeon.MAX_HEALTH - (2 * PigeonController.FIRE_DAMAGE), _pigeonController.GetHealth());
        }

        [Test]
        public void TestPigeonTakesDamageWhenCurrentTileIsNotOnFireAndOneNeighbourIsOnFire() {
            _tileController.IsOnFire().Returns(false);
            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile1.IsOnFire().Returns(true);

            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());
            _pigeonController.TakeFireDamage();

            // All relevant tiles were queried for onfire status
            _tileController.Received().IsOnFire();
            _neighbourTile0.Received().IsOnFire();
            _neighbourTile1.Received().IsOnFire();

            Assert.AreEqual(Pigeon.MAX_HEALTH - PigeonController.FIRE_DAMAGE, _pigeonController.GetHealth());
        }

        [Test]
        public void TestPigeonTakesDamageWhenCurrentTileIsOnFireAndOneNeighbourIsOnFire() {
            _tileController.IsOnFire().Returns(true);
            _neighbourTile0.IsOnFire().Returns(false);
            _neighbourTile1.IsOnFire().Returns(true);

            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());
            _pigeonController.TakeFireDamage();

            // All relevant tiles were queried for onfire status
            _tileController.Received().IsOnFire();
            _neighbourTile0.Received().IsOnFire();
            _neighbourTile1.Received().IsOnFire();

            // Takes double damage from tile it is standing on
            Assert.AreEqual(Pigeon.MAX_HEALTH - 3 * PigeonController.FIRE_DAMAGE, _pigeonController.GetHealth());
        }

        [Test]
        public void TestHealingPigeonNeverExceedsMaxHealth() {
            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());
            _pigeonController.Heal(50);
            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());
        }

        [Test]
        public void TestHealingPigeonWithPositiveValueAdjustsHealthAppropriately() {
            _pigeonController.InflictDamage(40);
            Assert.AreEqual(10, _pigeonController.GetHealth());
            
            _pigeonController.Heal(10);
            Assert.AreEqual(20, _pigeonController.GetHealth());    
        }

        [Test]
        public void TestHealingPigeonWithNegativeValueDoesNotAlterPigeonHealth() {
            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());
            _pigeonController.Heal(-50);
            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());

            _pigeonController.InflictDamage(30);
            _pigeonController.Heal(-60);
            Assert.AreEqual(Pigeon.MAX_HEALTH - 30, _pigeonController.GetHealth());
        }

        [Test]
        public void TestHurtingPigeonNeverDropsHealthBelowZero() {
            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());
            _pigeonController.InflictDamage(Pigeon.MAX_HEALTH * 2);
            Assert.AreEqual(0, _pigeonController.GetHealth());
        }

        [Test]
        public void TestHurtingPigeonWithNegativeValueDoesNotAlterPigeonHealth() {
            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());
            _pigeonController.InflictDamage(-50);
            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());
        }

        [Test]
        public void TestHurtingPigeonWithPositiveValueAdjustsHealthAppropriately() {
            Assert.AreEqual(Pigeon.MAX_HEALTH, _pigeonController.GetHealth());
            _pigeonController.InflictDamage(30);
            Assert.AreEqual(Pigeon.MAX_HEALTH - 30, _pigeonController.GetHealth());
        }

        [Test]
        public void TestWhenPigeonDiesFromFireDamageTileIsMarkedUnoccupied() {
            // Inflict some initial damage to the pigeon
            _pigeonController.InflictDamage(80);
            
            // Set tile and neighbouring tiles to be on fire
            _tileController.IsOnFire().Returns(true);
            _neighbourTile0.IsOnFire().Returns(true);
            _neighbourTile1.IsOnFire().Returns(true);

            // Pigeon will die from fire damage
            _pigeonController.TakeFireDamage();

            // Make sure the tile is marked as unoccupied
            _tileController.Received().MarkUnoccupied();
        }
    }
}