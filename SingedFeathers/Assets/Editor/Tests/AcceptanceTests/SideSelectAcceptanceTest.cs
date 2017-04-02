using System.Collections;
using Assets.Editor.TestTools.UITest.Test_Runner_Scripts;

namespace Assets.Editor.Tests.AcceptanceTests
{
    public class SideSelectAcceptanceTest : global::Assets.Editor.TestTools.UITest.Test_Runner_Scripts.UITest
    {
        [UISetUp]
        public IEnumerable SetUp() {
            // Load the scene we want.
            #if UNITY_EDITOR
                // The tests are being run through the editor
                yield return LoadSceneByPath("Assets/Scenes/SideSelectScene.unity");
            #elif !UNITY_EDITOR
                // The tests are being run on a device
                yield return LoadScene("SideSelectScene");
            #endif
        }

        [UITest]
        public IEnumerable TestAppearance() {
            yield return AssertLabel("Text", "What side do you choose?");

            yield return WaitFor(new ObjectAppeared("SavePigeonButton"));
            yield return WaitFor(new ObjectAppeared("BurnPigeonButton"));
            yield return WaitFor(new ObjectAppeared("Text"));
            yield return WaitFor(new ObjectAppeared("EventSystem"));
            yield return WaitFor(new ObjectAppeared("Canvas"));
            yield return WaitFor(new ObjectAppeared("Main Camera"));
        }

        [UITest]
        public IEnumerable TestSavePigeonButton() {
            // Press SavePigeonButton
            yield return Press("SavePigeonButton");

            // Should be at GameScene
            yield return WaitFor(new SceneLoaded("GameScene"));

            // Label should say...
            yield return AssertLabel("SideChosenText", "You have chosen to save the pigeons.");
        }

        [UITest]
        public IEnumerable TestBurnPigeonButton() {
            // Press SavePigeonButton
            yield return Press("BurnPigeonButton");

            // Should be at GameScene
            yield return WaitFor(new SceneLoaded("GameScene"));

            // Label should say...
            yield return AssertLabel("SideChosenText", "You have chosen to burn the pigeons.");
        }
    }
}