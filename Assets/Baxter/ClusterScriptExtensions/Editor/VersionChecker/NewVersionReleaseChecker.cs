using System;
using System.Linq;
using System.Threading.Tasks;
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
                    $"CS Extensions: Current version is latest",
                    $"Current version is the latest: {currentVersion}",
                    "Check Release Page",
                    "Close"
                );
            }
            else
            {
                openPage = EditorUtility.DisplayDialog(
                    $"CS Extensions: New Version Detected",
                    $"Current Version: {currentVersion}\n" +
                    $"Latest Version:{latestVersion}\n\n" + 
                    "Latest Version Release Note:\n\n" + desc,
                    "Open Release Page",
                    "Close"
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

            throw new Exception("Failed to get current installed version");
        }
        
        private static bool TryGetInstalledVersionFromUpm(out VersionNumber result)
        {
            var request = Client.List(true, true);
            Debug.Log("Check Unity Package Manager list...");
            while (!request.IsCompleted)
            {
                //waiting...
            }
            Debug.Log("Check Unity Package Manager list completed.");

            if (request.Status != StatusCode.Success)
            {
                throw new Exception("Failed to check current packages in project");
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
                throw new Exception("Failed to parse current package version");
            }

            Debug.Log($"UPM based CS Extensions version confirmed: {result}");
            return true;
        }

        private static bool TryGetInstalledVersionFromUnityPackage(out VersionNumber result)
        {
            var rawJson = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Baxter/ClusterScriptExtensions/package.json");
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
                throw new Exception("Failed to parse current version from package");
            }

            Debug.Log($".unitypackage file based CS Extensions version confirmed: {result}");
            return true;
        }
        
        private static async Task<(VersionNumber, string)> GetLatestReleaseInfoAsync()
        {
            Debug.Log("Check latest version of CS Extensions...");
            var request = UnityWebRequest.Get(LatestReleaseApiEndPoint);
            request.timeout = ApiTimeoutSecond;
            var operation = request.SendWebRequest();
            
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                throw new Exception("Failed to get latest release info from GitHub");
            }

            var rawInfo = request.downloadHandler.text;
            var releaseInfo = JsonUtility.FromJson<GitHubReleaseInfo>(rawInfo);

            if (!VersionNumber.TryParse(releaseInfo.Name, out var version))
            {
                throw new Exception("Failed to parse version number");
            }

            Debug.Log($"Latest version confirmed: {version}");
            return (version, releaseInfo.Body);
        }
    }
}
