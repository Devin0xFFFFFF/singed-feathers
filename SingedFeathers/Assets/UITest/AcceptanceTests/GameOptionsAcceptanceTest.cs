using Assets.UITest.Test_Runner_Scripts;
using System.Collections;

namespace Assets.UITest.AcceptanceTests {
    public class GameOptionsAcceptanceTest : Test_Runner_Scripts.UITest {
        [UISetUp]
        public IEnumerable SetUp() {
#if UNITY_EDITOR
            yield return LoadSceneByPath("Assets/Scenes/GameScene.unity");
#elif !UNITY_EDITOR
            yield return LoadScene("MapMakerScene");
#endif
        }

        [UITest]
        public IEnumerable TestAppearance() {
            yield return Press("OptionsButton");

            yield return WaitFor(new ObjectDisappeared("GameHUD"));
            yield return WaitFor(new ObjectAppeared("GameMenuCanvas"));
            yield return WaitFor(new ObjectAppeared("Panel"));
            yield return WaitFor(new ObjectAppeared("ReturnToTitleButton"));
            yield return WaitFor(new ObjectAppeared("OptionsText"));
            yield return WaitFor(new ObjectAppeared("HowToPlayButton"));
        }

        [UITest]
        public IEnumerable TestResumeButton() {
            yield return Press("OptionsButton");
            yield return WaitFor(new ObjectDisappeared("GameHUD"));
            yield return WaitFor(new ObjectAppeared("GameMenuCanvas"));

            yield return Press("ResumeButton");
            yield return WaitFor(new ObjectDisappeared("GameMenuCanvas"));
            yield return WaitFor(new ObjectAppeared("GameHUD"));
        }

        [UITest]
        public IEnumerable TestHowToPlayButton() {
            yield return Press("OptionsButton");
            yield return WaitFor(new ObjectDisappeared("GameHUD"));
            yield return WaitFor(new ObjectAppeared("GameMenuCanvas"));

            yield return Press("HowToPlayButton");
            yield return WaitFor(new ObjectDisappeared("GameMenuCanvas"));
            yield return WaitFor(new ObjectAppeared("HowToPlayCanvas"));

            yield return Press("BackButton");
            yield return WaitFor(new ObjectDisappeared("HowToPlayCanvas"));
            yield return WaitFor(new ObjectAppeared("GameMenuCanvas"));
        }

        [UITest]
        public IEnumerable TestReturnToTitleButton() {
            yield return Press("OptionsButton");
            yield return WaitFor(new ObjectDisappeared("GameHUD"));
            yield return WaitFor(new ObjectAppeared("GameMenuCanvas"));

            yield return Press("ReturnToTitleButton");
            yield return WaitFor(new ObjectDisappeared("GameMenuCanvas"));
            yield return WaitFor(new ObjectDisappeared("GameHUD"));

            yield return WaitFor(new SceneLoaded("GameSelectScene"));
        }
    }
}