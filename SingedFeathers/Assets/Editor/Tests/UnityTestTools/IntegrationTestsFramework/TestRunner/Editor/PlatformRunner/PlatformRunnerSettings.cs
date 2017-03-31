using Assets.Editor.TestTools.UnityTestTools.Common.Editor;

namespace Assets.Editor.TestTools.UnityTestTools.IntegrationTestsFramework.TestRunner.Editor.PlatformRunner
{
    public class PlatformRunnerSettings : ProjectSettingsBase
    {
        public string resultsPath;
        public bool sendResultsOverNetwork = true;
        public int port = 0;
    }
}
