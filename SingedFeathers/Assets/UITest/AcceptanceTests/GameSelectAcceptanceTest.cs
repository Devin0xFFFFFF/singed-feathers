using System.Collections;
using Assets.UITest.Test_Runner_Scripts;

namespace Assets.UITest.AcceptanceTests {
    public class GameSelectAcceptanceTest : UITest.Test_Runner_Scripts.UITest {
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
            yield return WaitFor(new ObjectAppeared("SinglePlayerButton"));
            yield return WaitFor(new ObjectAppeared("MultiPlayerButton"));
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
            yield return WaitFor(new ObjectAppeared("SinglePlayerButton"));
            yield return WaitFor(new ObjectAppeared("MultiPlayerButton"));
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
        public IEnumerable TestSinglePlayerButton() {
            // Press SinglePlayerButton
            yield return Press("SinglePlayerButton");

            // GameScene should be loaded
            yield return WaitFor(new SceneLoaded("MapSelectScene"));
        }

        [UITest]
        public IEnumerable TestMultiPlayerButton() {
            // Press MultiPlayerButton
            yield return Press("MultiPlayerButton");

            // Player should be prompted to enter a name
            yield return WaitFor(new ObjectAppeared("InputNameCanvas"));
            yield return WaitFor(new ObjectAppeared("ConfirmButton"));
            yield return WaitFor(new ObjectAppeared("CancelButton"));
            yield return WaitFor(new ObjectAppeared("InputField"));

            yield return WaitFor(new ObjectDisappeared("TitleCanvas"));

            // Click cancel
            yield return Press("CancelButton");
            yield return WaitFor(new ObjectDisappeared("InputNameCanvas"));
            yield return WaitFor(new ObjectAppeared("TitleCanvas"));
        }
    }
}