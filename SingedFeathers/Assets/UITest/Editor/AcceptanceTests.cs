using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tests {
    public class Run {
        [MenuItem("Tests/Run Unit Tests")]
        static void UnitTests() {
            UnityEditor.EditorTests.Batch.RunTestsInRunnerWindow ();
        }

        [MenuItem("Tests/Run Acceptance Tests/Editor")]
        static void EditorAcceptanceTests() {
            EditorApplication.isPlaying = true;
            EditorSceneManager.OpenScene ("Assets/UITest/TestRunner.unity");
        }

        [MenuItem("Tests/Run Acceptance Tests/WebGL")]
        static void WebGLAcceptanceTests() {
            helper_run(BuildTarget.WebGL, "AcceptanceRunner");
        }

        [MenuItem("Tests/Run Acceptance Tests/Android")]
        static void AndroidAcceptanceTests() {
            helper_run(BuildTarget.Android, "AcceptanceRunner.apk");
        }

        [MenuItem("Tests/Run Acceptance Tests/iOS")]
        static void iOSAcceptanceTests() {
            helper_run(BuildTarget.iOS, "AcceptanceRunner.app");
        }

        [MenuItem("Tests/Run Acceptance Tests/OSX Universal")]
        static void OSX_UniversalAcceptanceTests() {
            helper_run(BuildTarget.StandaloneOSXUniversal, "AcceptanceRunner");
        }

        [MenuItem("Tests/Run Acceptance Tests/OSX Intel")]
        static void OSX_IntelAcceptanceTests() {
            helper_run(BuildTarget.StandaloneOSXIntel, "AcceptanceRunner");
        }

        [MenuItem("Tests/Run Acceptance Tests/OSX Intel x64")]
        static void OSX_Intel64AcceptanceTests() {
            helper_run(BuildTarget.StandaloneOSXIntel64, "AcceptanceRunner");
        }

        [MenuItem("Tests/Run Acceptance Tests/Linux Universal")]
        static void Linux_UniversalAcceptanceTests() {
            helper_run(BuildTarget.StandaloneLinuxUniversal, "AcceptanceRunner");
        }

        [MenuItem("Tests/Run Acceptance Tests/Linux")]
        static void LinuxAcceptanceTests() {
            helper_run(BuildTarget.StandaloneLinux, "AcceptanceRunner");
        }

        [MenuItem("Tests/Run Acceptance Tests/Linux x64")]
        static void Linux64AcceptanceTests() {
            helper_run(BuildTarget.StandaloneLinux64, "AcceptanceRunner");
        }
            
        [MenuItem("Tests/Run Acceptance Tests/Windows")]
        static void WindowsAcceptanceTests() {
            helper_run(BuildTarget.StandaloneWindows, "AcceptanceRunner.exe");
        }

        [MenuItem("Tests/Run Acceptance Tests/Windows x64")]
        static void Windows64AcceptanceTests() {
            helper_run(BuildTarget.StandaloneWindows64, "AcceptanceRunner.exe");
        }

        static void helper_run(BuildTarget options, string executable) {
            ResolutionDialogSetting oldResolutionDialogSettings = PlayerSettings.displayResolutionDialog;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            buildOptions.target = options;
            buildOptions.options = BuildOptions.AutoRunPlayer;
            buildOptions.locationPathName = "AcceptanceRunner/" + executable;
            string[] scenes = { "Assets/UITest/TestRunner.unity", 
                "Assets/Scenes/GameSelectScene.unity", 
                "Assets/Scenes/GameScene.unity", 
                "Assets/Scenes/MapMakerScene.unity", 
                "Assets/Scenes/MapSelectScene.unity", 
                "Assets/Scenes/SideSelectScene.unity" };
            buildOptions.scenes = scenes;
            BuildPipeline.BuildPlayer(buildOptions);
            PlayerSettings.displayResolutionDialog = oldResolutionDialogSettings;
        }
    }
}