using System.Collections;
using Assets.Scripts.Views;
using CoreGame.Controllers.Interfaces;

namespace Assets.UITest.AcceptanceTests {
    public class GameAcceptanceTest : global::UITest {
        private IMapController _mapController;

        [UISetUp] public IEnumerable SetUp() {
            // Load the scene we want.
            #if UNITY_EDITOR
                // The tests are being run through the editor
                yield return LoadSceneByPath("Assets/Scenes/GameScene.unity");
                GameView gameView = FindObjectOfType<GameView>();

            #elif !UNITY_EDITOR
                // The tests are being run on a device
                yield return LoadScene("GameSelectScene");
            #endif
        }

        [UITest]
        public IEnumerable TestAppearance() {
            // Active
            yield return WaitFor(new ObjectAppeared("Main Camera"));
            yield return WaitFor(new ObjectAppeared("GameView"));
            yield return WaitFor(new ObjectAppeared("GameHUD"));
            yield return WaitFor(new ObjectAppeared("TurnCountLabel"));
            yield return WaitFor(new ObjectAppeared("PigeonCountLabel"));
            yield return WaitFor(new ObjectAppeared("FireButton"));
            yield return WaitFor(new ObjectAppeared("WaterButton"));
            yield return WaitFor(new ObjectAppeared("UndoButton"));
            yield return WaitFor(new ObjectAppeared("EndTurnButton"));
            yield return WaitFor(new ObjectAppeared("OptionsButton"));
            yield return WaitFor(new ObjectAppeared("SideChosenText"));
            yield return WaitFor(new ObjectAppeared("EventSystem"));
            yield return WaitFor(new ObjectAppeared("InputImage"));
            yield return WaitFor(new ObjectAppeared("Pigeon(Clone)"));

            // Inactive
            yield return WaitFor(new ObjectDisappeared("WaitingPanel"));
            yield return WaitFor(new ObjectDisappeared("GameMenuCanvas"));
            yield return WaitFor(new ObjectDisappeared("HowToPlayCanvas"));
            yield return WaitFor(new ObjectDisappeared("ActionNotAllowedText"));
        }

        [UITest]
        public IEnumerable TestFireButton() {
            yield return Press("FireButton");

            TileView fireTile = FindObjectOfType<TileView>();
            GameInputView inputView = FindObjectOfType<GameInputView>();
            inputView.HandleMapInput(fireTile);
            
            yield return WaitFor(new ObjectAppeared("RedTileSelectBorder(Clone)"));
        }

        [UITest]
        public IEnumerable TestWaterButton() {
            yield return Press("WaterButton");

            TileView fireTile = FindObjectOfType<TileView>();
            GameInputView inputView = FindObjectOfType<GameInputView>();
            inputView.HandleMapInput(fireTile);
            
            yield return WaitFor(new ObjectAppeared("BlueTileSelectBorder(Clone)"));
        }
    }
}