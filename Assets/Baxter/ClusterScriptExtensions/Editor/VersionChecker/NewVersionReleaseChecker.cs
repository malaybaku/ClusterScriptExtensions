using System;
using System.Linq;
using System.Threading.Tasks;
using Baxter.ClusterScriptExtensions.Editor.Localization;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

namespace Baxter.ClusterScriptExtensions.Editor.VersionChecker
{
    /// <summary>
    /// CS Extensionsの更新バージョンがリリースされているかどうかをチェックするクラス。
    /// </summary>
    public static class NewVersionReleaseChecker
    {
        private const string PackageName = "com.baxter.cluster-script-extensions";
        
        private const string LatestReleaseApiEndPoint = 
            "https://api.github.com/repos/malaybaku/ClusterScriptExtensions/releases/latest";
        private const int ApiTimeoutSecond = 3;

        private const string ReleasePageUrl = "https://github.com/malaybaku/ClusterScriptExtensions/releases";

        private const string UnityPackagePackageJsonAssetPath
            = "Assets/Baxter/ClusterScriptExtensions/package.json";
        
        [MenuItem("CS Extensions/Check Update...", priority = 15)]
        public static void CheckUpdate()
        {
            CheckUpdateInternalAsync();
        }

        private static async void CheckUpdateInternalAsync()
        {
            // いちおう明示的に例外キャッチしておく (あまり意味はないが)
            try
            {
                await CheckUpdateAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        private static async Task CheckUpdateAsync()
        {
            var (latestVersion, desc) = await GetLatestReleaseInfoAsync();
            var currentVersion = GetInstalledVersion();

            //NOTE: 最新バージョンであってもリリースページを見に行けるようにしておく
            var openPage = false;
            if (currentVersion.CompareTo(latestVersion) >= 0)
            {
                openPage = EditorUtility.DisplayDialog(
                    Texts.Get(Keys.VersionChecker_IsNewest_Title),
                    Texts.Get(Keys.VersionChecker_IsNewest_CurrentVersion, currentVersion),
                    Texts.Get(Keys.VersionChecker_IsNewest_CheckReleasePage),
                    Texts.Get(Keys.VersionChecker_Dialog_Close)
                );
            }
            else
            {
                openPage = EditorUtility.DisplayDialog(
                    Texts.Get(Keys.VersionChecker_NewVersionFound_Title),
                    Texts.Get(Keys.VersionChecker_NewVersionFound_CurrentVersion, currentVersion) + "\n" + 
                        Texts.Get(Keys.VersionChecker_NewVersionFound_LatestVersion, latestVersion) + "\n" + 
                        Texts.Get(Keys.VersionChecker_NewVersionFound_ReleaseNoteHeader) + "\n\n" + 
                        desc,
                    Texts.Get(Keys.VersionChecker_IsNewest_CheckReleasePage),
                    Texts.Get(Keys.VersionChecker_Dialog_Close)
                );
            }

            if (openPage)
            {
                Application.OpenURL(ReleasePageUrl);
            }
        }

        private static VersionNumber GetInstalledVersion()
        {
            if (TryGetInstalledVersionFromUpm(out var version))
            {
                return version;
            }

            if (TryGetInstalledVersionFromUnityPackage(out version))
            {
                return version;
            }

            throw new Exception(Texts.Get(Keys.VersionChecker_Failed_GetCurrentVersion));
        }
        
        private static bool TryGetInstalledVersionFromUpm(out VersionNumber result)
        {
            var request = Client.List(true, true);
            Debug.Log(Texts.Get(Keys.VersionChecker_UPM_List_Check));
            while (!request.IsCompleted)
            {
                //waiting...
            }
            Debug.Log(Texts.Get(Keys.VersionChecker_UPM_List_Check_Complete));
            
            if (request.Status != StatusCode.Success)
            {
                throw new Exception(Texts.Get(Keys.VersionChecker_UPM_Check_Failed_Unknown));
            }
            
            var package = request.Result.FirstOrDefault(pkg => pkg.name == PackageName);
            if (package == null)
            {
                // UPMではインストールしていなかったと解釈できるので、正常に取得失敗する
                result = VersionNumber.Zero;
                return false;
            }

            if (!VersionNumber.TryParse(package.version, out result))
            {
                // 見つかったパッケージのバージョンがパースできないのは変なので例外に帰着
                throw new Exception(Texts.Get(Keys.VersionChecker_UPM_Check_Failed_Parse));
            }

            Debug.Log(Texts.Get(Keys.VersionChecker_UPM_Check_Completed, result));
            return true;
        }

        private static bool TryGetInstalledVersionFromUnityPackage(out VersionNumber result)
        {
            var rawJson = AssetDatabase.LoadAssetAtPath<TextAsset>(UnityPackagePackageJsonAssetPath);
            if (rawJson == null)
            {
                // UPMでインストールしていると解釈できるので、正常に失敗扱いする
                result = VersionNumber.Zero;
                return false;
            }

            var versionData = JsonUtility.FromJson<PackageJsonData>(rawJson.text);
            if (!VersionNumber.TryParse(versionData.Version, out result))
            {
                // package.jsonがあるけどパースできないケースは例外に帰着する
                throw new Exception(Texts.Get(Keys.VersionChecker_UnityPackage_Failed_Parse));
            }

            Debug.Log(Texts.Get(Keys.VersionChecker_UnityPackage_Completed, result));
            return true;
        }
        
        private static async Task<(VersionNumber, string)> GetLatestReleaseInfoAsync()
        {
            Debug.Log(Texts.Get(Keys.VersionChecker_GetLatestVersion_Started));
            var request = UnityWebRequest.Get(LatestReleaseApiEndPoint);
            request.timeout = ApiTimeoutSecond;
            var operation = request.SendWebRequest();
            
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                throw new Exception(Texts.Get(Keys.VersionChecker_GetLatestVersion_Failed_Get));
            }

            var rawInfo = request.downloadHandler.text;
            var releaseInfo = JsonUtility.FromJson<GitHubReleaseInfo>(rawInfo);

            if (!VersionNumber.TryParse(releaseInfo.Name, out var version))
            {
                throw new Exception(Texts.Get(Keys.VersionChecker_GetLatestVersion_Failed_Parse));
            }

            Debug.Log(Texts.Get(Keys.VersionChecker_GetLatestVersion_Completed, version));
            return (version, releaseInfo.Body);
        }
    }
}
