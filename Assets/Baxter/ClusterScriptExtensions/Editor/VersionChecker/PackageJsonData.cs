using System;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.VersionChecker
{
    /// <summary>
    /// .unitypackage に同梱された package.json をTextAssetとして読み出したのをデシリアライズするために定義したクラス。
    /// </summary>
    [Serializable]
    public class PackageJsonData
    {
        //NOTE: authorとかもデータに含んでいるが、バージョンだけ分かればいいので無視
        [SerializeField] private string version;

        public string Version => version;
    }
}