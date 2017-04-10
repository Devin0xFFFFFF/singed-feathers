using System.Collections;
using Assets.UITest.Test_Runner_Scripts;
using Assets.Scripts.Service.IO;
using UnityEngine.Assertions;

namespace Assets.UITest.AcceptanceTests {
    public class GameServiceTest : Test_Runner_Scripts.UITest {

        private int _response;
        private bool _finished;
        private GameIO _gameIO;

        [UISetUp]
        public void SetUp() {
            _finished = false;
            _gameIO = new GameIO();
        }

        [UITest]
        public IEnumerable TestGameService() {
            yield return StartCoroutine(_gameIO.Test(delegate (int responseCode) {
                _response = responseCode;
                _finished = true;
            }));

            yield return WaitFor(new BoolCondition(IsDone));
            Assert.AreEqual(_response, 0);
        }

        public bool IsDone() { return _finished; }
    }
}