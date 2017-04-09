using System.Collections;
using System.Linq;
using Assets.Scripts.Views;
using Assets.UITest.Test_Runner_Scripts;
using CoreGame.Models;
using UnityEngine;
using Assets.Scripts.Service;
using UnityEngine.Assertions;

namespace Assets.UITest.AcceptanceTests {
    public class GameServiceTest : Test_Runner_Scripts.UITest {

        private int _response;
        private bool _finished;
        private GameServiceIO _gameServiceIO;

        [UISetUp]
        public void SetUp() {
            _finished = false;
            _gameServiceIO = new GameServiceIO();
        }

        [UITest]
        public IEnumerable TestGameService() {
            yield return StartCoroutine(_gameServiceIO.Test(delegate (int responseCode) {
                _response = responseCode;
                _finished = true;
            }));

            yield return WaitFor(new BoolCondition(IsDone));
            Assert.AreEqual(_response, 0);
        }

        public bool IsDone() {
            return _finished;
        }
    }
}