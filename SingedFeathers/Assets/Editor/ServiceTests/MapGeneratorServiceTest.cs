using Assets.Scripts.Models;
using Assets.Scripts.Service;
using NUnit.Framework;

namespace Assets.Editor.ServiceTests {
    [TestFixture]
    public class MapGeneratorServiceTest {
        private MapGeneratorService _mapGeneratorService;

        [SetUp]
        public void Init() {
            _mapGeneratorService = new MapGeneratorService();
        }

        [Test]
        public void TestIfNoMapFoundReturnsNull() {
            Map map = _mapGeneratorService.GenerateMap(-1);
            Assert.Null(map);
        }
    }
}