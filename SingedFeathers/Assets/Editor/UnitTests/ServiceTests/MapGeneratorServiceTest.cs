using CoreGame.Models;
using CoreGame.Service;
using NUnit.Framework;

namespace Assets.Editor.UnitTests.ServiceTests {
    [TestFixture]
    public class MapGeneratorServiceTest {
        private MapGeneratorService _mapGeneratorService;

        [SetUp]
        public void Init() { _mapGeneratorService = new MapGeneratorService(); }

        [Test]
        public void TestIfNoMapFoundReturnsNull() {
            Map map = _mapGeneratorService.GenerateMap(null);
            Assert.Null(map);
        }

        [Test]
        public void TestIfInvalidSerializedMapReturnsNull() {
            Map map = _mapGeneratorService.GenerateMap("{");
            Assert.Null(map);
        }

        [Test]
        public void TestIfDefaultMapIsNotNull() {
            Map map = _mapGeneratorService.GenerateDefaultMap(0, 0);
            Assert.Null(map);

            map = _mapGeneratorService.GenerateDefaultMap(0, 10);
            Assert.Null(map);

            map = _mapGeneratorService.GenerateDefaultMap(10, 0);
            Assert.Null(map);

            map = _mapGeneratorService.GenerateDefaultMap(10, 10);
            Assert.NotNull(map);
            Assert.NotNull(map.InitialFirePositions);
            Assert.NotNull(map.InitialPigeonPositions);
            Assert.NotNull(map.Pigeons);
        }
    }
}