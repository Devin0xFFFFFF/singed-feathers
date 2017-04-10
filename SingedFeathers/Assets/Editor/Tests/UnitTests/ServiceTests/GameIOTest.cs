using Assets.Scripts.Service.Client;
using Assets.Scripts.Service.IO;
using NUnit.Framework;

namespace Assets.Editor.Tests.UnitTests.ServiceTests {
    [TestFixture]
    public class GameIOTest {
        private GameIO _gameIO;

        [SetUp]
        public void Init() { _gameIO = new GameIO(); }

        [Test]
        public void TestParseCommitTurnResult() {
            string responseBody = "true";
            ClientResult result = new ClientResult("CommitTurn", 200, responseBody, false, null);
            var parsedResult = _gameIO.ParseCommitTurnResult(result);
            Assert.IsTrue(parsedResult);
        }

        [Test]
        public void TestParsePollGameResult() {
            string responseBody = "{ IsValid: true, Turn: null }";
            ClientResult result = new ClientResult("PollGame", 200, responseBody, false, null);
            var parsedResult = _gameIO.ParsePollGameResult(result);
            Assert.NotNull(parsedResult);
            Assert.IsTrue(parsedResult.IsValid);
            Assert.IsNull(parsedResult.Turn);
        }

        [Test]
        public void TestParseSurrenderResult() {
            string responseBody = "{ IsValid: true, Turn: null }";
            ClientResult result = new ClientResult("Surrender", 200, responseBody, false, null);
            var parsedResult = _gameIO.ParseSurrenderResult(result);
            Assert.NotNull(parsedResult);
            Assert.IsTrue(parsedResult.IsValid);
            Assert.IsNull(parsedResult.Turn);
        }
    }
}