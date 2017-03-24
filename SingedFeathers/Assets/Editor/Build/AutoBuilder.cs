using System;
using UnityEditor;

namespace Assets.Editor.Build {
	public static class AutoBuilder {
		// Source: http://wiki.unity3d.com/index.php?title=AutoBuilder

		public static string[] GetScenePaths() {
			string[] scenes = new string[EditorBuildSettings.scenes.Length];

			for(int i = 0; i < scenes.Length; i++) {
				scenes[i] = EditorBuildSettings.scenes[i].path;
			}

			return scenes;
		}

		[MenuItem("File/AutoBuilder/Android")]
		public static void PerformAndroidBuild() {
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
			BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Android/SingedFeathers.apk", BuildTarget.Android, BuildOptions.Development);
		}

		[MenuItem("File/AutoBuilder/iOS")]
		public static void PerformiOSBuild() {
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
			BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/iOS", BuildTarget.iOS, BuildOptions.None);
		}

		[MenuItem("File/AutoBuilder/Web")]
		public static void PerformWebBuild() {
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WebGL);
			BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Web", BuildTarget.WebGL, BuildOptions.None);
		}

		[MenuItem("File/AutoBuilder/GenerateProjects")]
		public static void GenerateProjects() {
			EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
		}
	}
}

