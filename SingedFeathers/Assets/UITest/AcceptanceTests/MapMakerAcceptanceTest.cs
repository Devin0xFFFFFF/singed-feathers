using System.Collections;

namespace Assets.UITest.AcceptanceTests {
    public class MapMakerAcceptanceTest : global::UITest {
        [UISetUp]
        public IEnumerable SetUp()
        {
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

    }
}