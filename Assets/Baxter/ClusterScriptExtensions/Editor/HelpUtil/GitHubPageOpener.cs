using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.HelpUtil
{
    public static class GitHubPageOpener
    {
        [MenuItem("CS Extensions/Open Basic Setup Help (GitHub)", priority = 11)]
        public static void OpenBasicSetupPage()
        {
            Application.OpenURL("https://github.com/malaybaku/ClusterScriptExtensions/blob/main/README.md");
        }

        [MenuItem("CS Extensions/Open Script Examples (GitHub)", priority = 12)]
        public static void OpenScriptExamplesPage()
        {
            Application.OpenURL("https://github.com/malaybaku/ClusterScriptExtensions/blob/main/ScriptExamples.md");
        }
    }
}
