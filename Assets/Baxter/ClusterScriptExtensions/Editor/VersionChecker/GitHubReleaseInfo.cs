using System;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.VersionChecker
{
    /// <summary>
    /// GitHubのRelease情報から取得可能な、リリースバージョン名とリリース情報のテキストからなる値
    /// </summary>
    [Serializable]
    public class GitHubReleaseInfo
    {
        [SerializeField] private string name;
        [SerializeField] private string body;

        public string Name => name;
        public string Body => body;
    }
}