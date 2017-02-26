using Assets.Scripts.Controllers;
using NUnit.Framework;

namespace SingedFeathers.Test.ControllerTest {
    [TestFixture]
    public class MapControllerTest {
        private MapController _mapController;

        [SetUp]
        public void Init() { _mapController = new MapController(); }

        [Test]
        public void TestGenerateInitializesProperly() {
            _mapController.GenerateMap();
            Assert.AreEqual(5, _mapController.Height);
            Assert.AreEqual(5, _mapController.Width);
        }

        [Test]
        public void TestApplyHeatDoesNotThrowOutOFBoundsException() {
            _mapController.ApplyHeat(-1, -1);
            _mapController.ApplyHeat(-1, 0);
            _mapController.ApplyHeat(0, -1);

        }
    }
}
