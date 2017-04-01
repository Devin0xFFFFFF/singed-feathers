using System.Collections;

namespace Assets.UITest.AcceptanceTests {
    public class MapMakerOptionsAcceptanceTest : global::UITest {
        [UISetUp]
        public IEnumerable SetUp() {
            // Load the scene we want.
            #if UNITY_EDITOR
                // The tests are being run through the editor
                yield return LoadSceneByPath("Assets/Scenes/MapMakerScene.unity");
            #elif !UNITY_EDITOR
                // The tests are being run on a device
                yield return LoadScene("MapMakerScene");
            #endif
        }

        [UITest]
        public IEnumerable TestAppearance() {
            // Access Options
            yield return Press("OptionsButton");

            // Make sure MenuCanvas and children appeared
            yield return WaitFor(new ObjectAppeared("MenuCanvas"));
            yield return WaitFor(new ObjectAppeared("Panel"));
            yield return WaitFor(new ObjectAppeared("RestartButton"));
            yield return WaitFor(new ObjectAppeared("ReturnToTitleButton"));
            yield return WaitFor(new ObjectAppeared("OptionsText"));
            yield return WaitFor(new ObjectAppeared("HowToButton"));
        }

        [UITest]
        public IEnumerable TestResumeButton() {
            // Access Options
            yield return Press("OptionsButton");

            // return to map editing
            yield return Press("ResumeButton");

            // Make sure the MenuCanvas and assumed children are inactive
            yield return WaitFor(new ObjectDisappeared("MenuCanvas"));
            yield return WaitFor(new ObjectDisappeared("Panel"));
            yield return WaitFor(new ObjectDisappeared("RestartButton"));
            yield return WaitFor(new ObjectDisappeared("ReturnToTitleButton"));
            yield return WaitFor(new ObjectDisappeared("OptionsText"));
            yield return WaitFor(new ObjectDisappeared("HowToButton"));
        }

        [UITest]
        public IEnumerable TestReturnToTitleButton() {
            // Access Options
            yield return Press("OptionsButton");

            // Return to title
            yield return Press("ReturnToTitleButton");

            // Make sure it worked!
            yield return WaitFor(new SceneLoaded("GameSelectScene"));
        }

        [UITest]
        public IEnumerable TestHowToButton() {
            // Access Options
            yield return Press("OptionsButton");

            // Access How to play canvas
            yield return Press("HowToButton");

            // Make sure HelpCanvas and children are active
            yield return WaitFor(new ObjectAppeared("HelpCanvas"));
            yield return WaitFor(new ObjectAppeared("Panel"));
            yield return WaitFor(new ObjectAppeared("HelpTitle"));
            yield return WaitFor(new ObjectAppeared("HelpBodyText"));

            // Other UI elements should be inactive
            yield return WaitFor(new ObjectDisappeared("MapMakerCanvas"));
            yield return WaitFor(new ObjectDisappeared("MenuCanvas"));

            // Go back to options
            yield return Press("Button");

            // Help canvas and assumed childre should be inactive
            // Make sure HelpCanvas and children are active
            yield return WaitFor(new ObjectDisappeared("HelpCanvas"));
            yield return WaitFor(new ObjectDisappeared("HelpTitle"));
            yield return WaitFor(new ObjectDisappeared("HelpBodyText"));

            // MeunuCanvas should be active again
            yield return WaitFor(new ObjectAppeared("MenuCanvas"));

            // MapMakerCanvas should still be incative
            yield return WaitFor(new ObjectDisappeared("MapMakerCanvas"));
        }
    }
}