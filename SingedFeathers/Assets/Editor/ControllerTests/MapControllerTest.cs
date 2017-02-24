using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using Assets.Scripts.Service;
using NUnit.Framework;
using NSubstitute;

namespace Assets.Editor.ControllerTests {

    [TestFixture()]
    public class MapControllerTest {
        private MapController _mapController;
        private ITileController _tile1;
        private ITileController _tile2;
        private ITileController _tile3;

        [SetUp]
        public void Init() {
            IMapGeneratorService mapGenerator = Substitute.For<IMapGeneratorService>();
            mapGenerator.GenerateMap(Arg.Any<int>()).Returns(GenerateTestMap());
            _mapController = new MapController(mapGenerator);
        }

        [Test]
        public void TestGenerateInitializesProperly() {
            _mapController.GenerateMap();
            Assert.AreEqual(3, _mapController.Height);
            Assert.AreEqual(1, _mapController.Width);
        }

        [Test]
        public void TestApplyHeatDoesNotThrowOutOFBoundsException() {
            _mapController.ApplyHeat(-1, -1);
            _mapController.ApplyHeat(-1, 0);
            _mapController.ApplyHeat(0, -1);

        }

        private Map GenerateTestMap() {
            ITileController[,] tileControllers = IntializeControllers();
            return new Map() {
                Height = 3,
                Width = 1,
                InitialFirePosition = new Position() { X = 1, Y = 0 },
                TileMap = tileControllers
            };
        }

        private ITileController[,] IntializeControllers() {
            _tile1 = Substitute.For<ITileController>();
            _tile2 = Substitute.For<ITileController>();
            _tile3 = Substitute.For<ITileController>();

            ITileController[,] tileControllers = {
                { _tile1, _tile2, _tile3 }
            };
            return tileControllers;
        }
    }
}
