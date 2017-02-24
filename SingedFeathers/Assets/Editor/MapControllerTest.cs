using Assets.Scripts.Controllers;
using NUnit.Framework;

namespace Assets.Editor {
    [TestFixture()]
    public class MapControllerTest {
        private MapController _mapController;

        [SetUp]
        public void Init() {
            _mapController = new MapController();
        }

        [Test]
        public void Generate_InitializesProperly() {
            _mapController.GenerateMap();
            Assert.AreEqual(5, _mapController.Height);
            Assert.AreEqual(5, _mapController.Width);
        }
    }
}
