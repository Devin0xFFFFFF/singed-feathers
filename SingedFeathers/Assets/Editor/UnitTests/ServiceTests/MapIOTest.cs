using Assets.Scripts.Service.Client;
using Assets.Scripts.Service.IO;
using NUnit.Framework;

namespace Assets.Editor.UnitTests.ServiceTests {
    [TestFixture]
    public class MapIOTest {
        private MapIO _mapIO;

        [SetUp]
        public void Init() { _mapIO = new MapIO(); }

        [Test]
        public void TestParseCreateMapResult() {
            ClientResult result = new ClientResult("CreateMap", 200, "", false, null);
            var parsedResult = _mapIO.ParseCreateMapResult(result);
            Assert.NotNull(parsedResult);
        }

        [Test]
        public void TestParseGetMapDataResult() {
            ClientResult result = new ClientResult("GetMapData", 200, "", false, null);
            var parsedResult = _mapIO.ParseGetMapDataResult(result);
            Assert.NotNull(parsedResult);
        }

        [Test]
        public void TestParseGetMapsResult() {
            string responseBody = "{ Maps: [] }";
            ClientResult result = new ClientResult("GetMaps", 200, responseBody, false, null);
            var parsedResult = _mapIO.ParseGetMapsResult(result);
            Assert.NotNull(parsedResult);
        }
    }
}