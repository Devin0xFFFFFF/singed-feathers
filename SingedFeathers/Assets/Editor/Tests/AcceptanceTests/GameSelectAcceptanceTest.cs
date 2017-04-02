using System.Collections;
using Assets.Editor.TestTools.UITest.Test_Runner_Scripts;

namespace Assets.Editor.Tests.AcceptanceTests {
    public class GameSelectAcceptanceTest : global::Assets.Editor.TestTools.UITest.Test_Runner_Scripts.UITest {
        [UISetUp]
        public IEnumerable SetUp() {
            // Load the scene we want.
            #if UNITY_EDITOR
                // The tests are being run through the editor
                yield return LoadSceneByPath("Assets/Scenes/GameSelectScene.unity");
            #elif !UNITY_EDITOR
                // The tests are being run on a device
                yield return LoadScene("GameSelectScene");
            #endif
        }

        [UITest]
        public IEnumerable TestAppearanceOnLoad() {
            // The camera should be active
            yield return WaitFor(new ObjectAppeared("MainCamera"));

            // The TitleCanvas should be active.
            yield return WaitFor(new ObjectAppeared("TitleCanvas"));

            // The TitleCanvas' children should also be active...
            yield return WaitFor(new ObjectAppeared("GameTitle"));
            yield return WaitFor(new ObjectAppeared("PlayGameButton"));
            yield return WaitFor(new ObjectAppeared("MapMakerButton"));
            yield return WaitFor(new ObjectAppeared("HowToPlayButton"));

            // HelpCanvas should not be active.
            yield return WaitFor(new ObjectDisappeared("HelpCanvas")); 
        }

        [UITest]
        public IEnumerable TestHowToPlayButton() {
            // Press button "HowToPlayButton", if not active then timeout will occur.
            yield return Press("HowToPlayButton");

            // TitleCanvas should not be active.
            yield return WaitFor(new ObjectDisappeared("TitleCanvas"));

            // HelpCanvas should be active, if not timeout will occur.
            yield return WaitFor(new ObjectAppeared("HelpCanvas"));

            // So should all of HelpCanvas' children...
            yield return WaitFor(new ObjectAppeared("HowToPlayText"));
            yield return WaitFor(new ObjectAppeared("TutorialBody"));

            // if BackButton is active, press it...
            yield return Press("BackButton");

            // Everything should be as it were on startup so test for that...

            // HelpCanvas should not be active now.
            yield return WaitFor(new ObjectDisappeared("HelpCanvas"));

            // The TitleCanvas should be active.
            yield return WaitFor(new ObjectAppeared("TitleCanvas"));

            // The TitleCanvas' children should also be active...
            yield return WaitFor(new ObjectAppeared("GameTitle"));
            yield return WaitFor(new ObjectAppeared("PlayGameButton"));
            yield return WaitFor(new ObjectAppeared("MapMakerButton"));
            yield return WaitFor(new ObjectAppeared("HowToPlayButton"));

            // HelpCanvas should not be active.
            yield return WaitFor(new ObjectDisappeared("HelpCanvas")); 
        }

        [UITest]
        public IEnumerable TestMapMakerButton() {
            // Press MapMakerButton
            yield return Press("MapMakerButton");

            // MapMakerScene should be loaded.
            yield return WaitFor(new SceneLoaded("MapMakerScene"));
        }

        [UITest]
        public IEnumerable TestPlayGameButton() {
            // Press PlayGameButton
            yield return Press("PlayGameButton");

            // GameScene should be loaded
            yield return WaitFor(new SceneLoaded("MapSelectScene"));
        }
    }
}