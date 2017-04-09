using System.Collections;
using Assets.UITest.Test_Runner_Scripts;
using UnityEngine;

namespace Assets.UITest.AcceptanceTests {
    public class SideSelectAcceptanceTest : Test_Runner_Scripts.UITest {
        [UISetUp]
        public IEnumerable SetUp() {
            PlayerPrefs.SetInt("NumPlayers", 1);
#if UNITY_EDITOR
            yield return LoadSceneByPath("Assets/Scenes/SideSelectScene.unity");
#elif !UNITY_EDITOR
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