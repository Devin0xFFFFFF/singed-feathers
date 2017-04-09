using Assets.Scripts.Service.Client;
using Assets.Scripts.Service.IO;
using NUnit.Framework;
using System;

namespace Assets.Editor.UnitTests.ServiceTests {
    [TestFixture]
    public class LobbyIOTest {
        private LobbyIO _lobbyIO;

        [SetUp]
        public void Init() { _lobbyIO = new LobbyIO(); }

        [Test]
        public void TestParseCreateLobbyResult() {
            ClientResult result = new ClientResult("CreateLobby", 200, "", false, null);
            var parsedResult = _lobbyIO.ParseCreateLobbyResult(result);
            Assert.NotNull(parsedResult);
        }

        [Test]
        public void TestParseGetLobbiesResult() {
            string responseBody = "{ Lobbies: [] }";
            ClientResult result = new ClientResult("GetLobbies", 200, responseBody, false, null);
            var parsedResult = _lobbyIO.ParseGetLobbiesResult(result);
            Assert.NotNull(parsedResult);
        }

        [Test]
        public void TestParseJoinLobbyResult() {
            string responseBody = "{ ResultCode: 0, ResultMessage: \"\" }";
            ClientResult result = new ClientResult("JoinLobby", 200, responseBody, false, null);
            var parsedResult = _lobbyIO.ParseJoinLobbyResult(result);
            Assert.NotNull(parsedResult);
            Assert.AreEqual(0, parsedResult.ResultCode);
            Assert.NotNull(parsedResult.ResultMessage);
        }

        [Test]
        public void TestParseLeaveLobbyResult() {
            string responseBody = "{ ResultCode: 0, ResultMessage: \"\" }";
            ClientResult result = new ClientResult("LeaveLobby", 200, responseBody, false, null);
            var parsedResult = _lobbyIO.ParseLeaveLobbyResult(result);
            Assert.NotNull(parsedResult);
            Assert.AreEqual(0, parsedResult.ResultCode);
            Assert.NotNull(parsedResult.ResultMessage);
        }

        [Test]
        public void TestParseReadyLobbyResult() {
            string responseBody = "{ ResultCode: 0, ResultMessage: \"\" }";
            ClientResult result = new ClientResult("ReadyLobby", 200, responseBody, false, null);
            var parsedResult = _lobbyIO.ParseLeaveLobbyResult(result);
            Assert.NotNull(parsedResult);
            Assert.AreEqual(0, parsedResult.ResultCode);
            Assert.NotNull(parsedResult.ResultMessage);
        }

        [Test]
        public void TestParsePollLobbyResult() {
            string responseBody = "{ ResultCode: 0, ResultMessage: \"\" }";
            ClientResult result = new ClientResult("PollLobby", 200, responseBody, false, null);
            var parsedResult = _lobbyIO.ParseLeaveLobbyResult(result);
            Assert.NotNull(parsedResult);
            Assert.AreEqual(0, parsedResult.ResultCode);
            Assert.NotNull(parsedResult.ResultMessage);
        }
    }
}