using System.Collections;
using System.Linq;
using Assets.Scripts.Views;
using UnityEngine.UI;

namespace Assets.UITest.AcceptanceTests {
    public class MapMakerAcceptanceTest : global::UITest {
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
        public IEnumerable TestAppearanceOnLoad() {
            // Make sure things are active
            yield return WaitFor(new ObjectAppeared("Main Camera"));
            yield return WaitFor(new ObjectAppeared("MapMakerView"));
            yield return WaitFor(new ObjectAppeared("MapMakerCanvas"));
            yield return WaitFor(new ObjectAppeared("OptionsButton"));
            yield return WaitFor(new ObjectAppeared("AddFireButton"));
            yield return WaitFor(new ObjectAppeared("AddPigeonButton"));
            yield return WaitFor(new ObjectAppeared("UploadButton"));
            yield return WaitFor(new ObjectAppeared("TileSelectPanel"));
            yield return WaitFor(new ObjectAppeared("TileSelectList"));
            yield return WaitFor(new ObjectAppeared("RemoveButton"));
            yield return WaitFor(new ObjectAppeared("MapMakerText"));
            yield return WaitFor(new ObjectAppeared("ChangeTileText"));
            yield return WaitFor(new ObjectAppeared("NumTurnsSlider"));
            yield return WaitFor(new ObjectAppeared("Background"));
            yield return WaitFor(new ObjectAppeared("Fill Area"));
            yield return WaitFor(new ObjectAppeared("Fill"));
            yield return WaitFor(new ObjectAppeared("Handle Slide Area"));
            yield return WaitFor(new ObjectAppeared("Handle"));
            yield return WaitFor(new ObjectAppeared("NumberOfTurnsText"));
            yield return WaitFor(new ObjectAppeared("NumTurnsValue"));
            yield return WaitFor(new ObjectAppeared("EventSystem"));

            // Make sure the following is inactive
            yield return WaitFor(new ObjectDisappeared("MenuCanvas"));
            yield return WaitFor(new ObjectDisappeared("HelpCanvas"));
            yield return WaitFor(new ObjectDisappeared("UploadMapCanvas"));
        }

        [UITest]
        public IEnumerable TestOptionsButton() {
            //More detailed tests for the Options Menu is in MapMakerOptionsAcceptanceTest.cs
            // Access Options
            yield return Press("OptionsButton");

            // Make sure MenuCanvas and children appeared
            yield return WaitFor(new ObjectAppeared("MenuCanvas"));
        }

        [UITest]
        public IEnumerable TestUploadButton() {
            // Test only for active elements
            // Press UploadButton
            yield return Press("UploadButton");

            // Check that UploadMapCanvas and assumed children are active
            yield return WaitFor(new ObjectAppeared("UploadMapCanvas"));
            yield return WaitFor(new ObjectAppeared("MapTitle"));
            yield return WaitFor(new ObjectAppeared("AuthorNameInput"));
            yield return WaitFor(new ObjectAppeared("AuthorName"));
            yield return WaitFor(new ObjectAppeared("MapTitleInput"));
            yield return WaitFor(new ObjectAppeared("XButton"));

            // ResultText should be inactive
            yield return WaitFor(new ObjectDisappeared("ResultText"));

            // Press Cancel
            yield return Press("CancelButton");

            // UploadCanvas should be inactive
            yield return WaitFor(new ObjectDisappeared("UploadMapCanvas"));
        }

        [UITest]
        public IEnumerable TestUploadWithNoPigeons() {
            // Test only for active elements
            // Press UploadButton
            yield return Press("UploadButton");

            yield return WaitFor(new ObjectAppeared("UploadMapCanvas"));

            yield return Press("UploadToServerButton");
            yield return WaitFor(new ObjectAppeared("ResultText"));
            yield return AssertLabel("ResultText", "You need at least one pigeon in your map!");
            
            // Press Cancel
            yield return Press("CancelButton");

            // UploadCanvas should be inactive
            yield return WaitFor(new ObjectDisappeared("UploadMapCanvas"));
        }

        [UITest]
        public IEnumerable TestAddFireButton() {
            yield return Press("AddFireButton");

            TileView fireTile = FindObjectOfType<TileView>();
            MapMakerInputView inputView = FindObjectOfType<MapMakerInputView>();
            inputView.HandleMapInput(fireTile);

            yield return WaitFor(new ObjectAppeared("FireSprite"));
        }

        [UITest]
        public IEnumerable TestAddPigeonButton() {
            yield return Press("AddPigeonButton");

            TileView grassTile = FindObjectOfType<TileView>();
            MapMakerInputView inputView = FindObjectOfType<MapMakerInputView>();
            inputView.HandleMapInput(grassTile);

            yield return WaitFor(new ObjectAppeared("Pigeon(Clone)"));
        }

        [UITest]
        public IEnumerable TestChangingtileType() {
            Button[] buttons = FindObjectsOfType<Button>();
            Button stoneButton = buttons.FirstOrDefault(x => x.name.Equals("TileButton(Clone)"));

            yield return Press(stoneButton.gameObject);

            TileView tile = FindObjectOfType<TileView>();
            MapMakerInputView inputView = FindObjectOfType<MapMakerInputView>();
            inputView.HandleMapInput(tile);

            yield return WaitFor(new ObjectAppeared("NonFlammableStoneTIle(Clone)"));
        }

        [UITest]
        public IEnumerable TestRemoveButton() {
            // Add fire to the map
            yield return Press("AddFireButton");
            TileView tile = FindObjectOfType<TileView>();
            MapMakerInputView inputView = FindObjectOfType<MapMakerInputView>();
            inputView.HandleMapInput(tile);
            yield return WaitFor(new ObjectAppeared("FireSprite"));

            // Remove the fire
            yield return Press("RemoveButton");
            inputView.HandleMapInput(tile);
            yield return WaitFor(new ObjectDisappeared("FireSprite"));

            // Add pigeon to the map
            yield return Press("AddPigeonButton");
            inputView.HandleMapInput(tile);
            yield return WaitFor(new ObjectAppeared("Pigeon(Clone)"));

            // Remove the pigeon
            yield return Press("RemoveButton");
            inputView.HandleMapInput(tile);
            yield return WaitFor(new ObjectDisappeared("Pigeon(Clone)"));

            // Change tile type
            Button[] buttons = FindObjectsOfType<Button>();
            Button stoneButton = buttons.FirstOrDefault(x => x.name.Equals("TileButton(Clone)"));
            yield return Press(stoneButton.gameObject);
            inputView.HandleMapInput(tile);
            yield return WaitFor(new ObjectAppeared("NonFlammableStoneTIle(Clone)"));

            // Reset the tile
            yield return Press("RemoveButton");
            inputView.HandleMapInput(tile);
            yield return WaitFor(new ObjectDisappeared("NonFlammableStoneTIle(Clone)"));
        }
    }
}