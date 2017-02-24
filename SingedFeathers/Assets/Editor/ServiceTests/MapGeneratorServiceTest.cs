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

        public void TestMapGeneration() {
            _mapGeneratorService.GenerateMap(1);
            //...do more things... is this one even testable?
        }
    }
}
