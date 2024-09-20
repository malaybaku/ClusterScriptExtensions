using System.Collections.Generic;

namespace Baxter.ClusterScriptExtensions.Editor.Localization
{
    public enum Keys
    {
        ScriptableItemExtension_Inspector_ReloadScript,
        ScriptableItemExtension_Inspector_ResetValues,

        ScriptExtensionField_UnsupportedType,
        ScriptExtensionField_HumanoidAnimation_NotHumanoid,
        ScriptExtensionField_WorldItem_NotInScene,
        ScriptExtensionField_WorldItemReferece_NotPrefab,
        ScriptExtensionField_Material_IrrelatedToItem,
        
        ScriptExtensionField_Warning_FailedToEstimateType,
        
        ItemScriptUpdater_Location_Invalid,
        SceneScriptUpdater_SceneCountInvalid,
        SceneScriptUpdater_Log_UpdateStart,
        SceneScriptUpdater_Log_UpdateCompleted,
        
        VersionChecker_IsNewest_Title,
        VersionChecker_IsNewest_CurrentVersion,
        VersionChecker_IsNewest_CheckReleasePage,

        VersionChecker_NewVersionFound_Title,
        VersionChecker_NewVersionFound_CurrentVersion,
        VersionChecker_NewVersionFound_LatestVersion,
        VersionChecker_NewVersionFound_ReleaseNoteHeader,
        VersionChecker_NewVersionFound_OpenReleasePage,
        
        VersionChecker_Dialog_Close,
        
        VersionChecker_Failed_GetCurrentVersion,
        
        VersionChecker_UPM_List_Check,
        VersionChecker_UPM_List_Check_Complete,
        VersionChecker_UPM_Check_Failed_Unknown,
        VersionChecker_UPM_Check_Failed_Parse,
        VersionChecker_UPM_Check_Completed,
        
        VersionChecker_UnityPackage_Failed_Parse,
        VersionChecker_UnityPackage_Completed,
        
        VersionChecker_GetLatestVersion_Started,
        VersionChecker_GetLatestVersion_Failed_Get,
        VersionChecker_GetLatestVersion_Failed_Parse,
        VersionChecker_GetLatestVersion_Completed,
        
    }

    public static class Texts
    {
        private static readonly Dictionary<Keys, string> EnglishTexts = new()
        {
            [Keys.ScriptableItemExtension_Inspector_ReloadScript] = "Reload Script",
            [Keys.ScriptableItemExtension_Inspector_ResetValues] = "Reset Values",

            [Keys.ScriptExtensionField_UnsupportedType] = "(error: unsupported type!)",
            [Keys.ScriptExtensionField_HumanoidAnimation_NotHumanoid] = "Error: The clip is not for Humanoid. Please set Humanoid animation.",
            [Keys.ScriptExtensionField_WorldItem_NotInScene] = "Error: Please set item in scene.",
            [Keys.ScriptExtensionField_WorldItemReferece_NotPrefab] = "Error: Please set item prefab, which is not in scene.",
            [Keys.ScriptExtensionField_Material_IrrelatedToItem] = "Error: Please set material used by item or its children",

            [Keys.ScriptExtensionField_Warning_FailedToEstimateType] = "Failed to estimate field type: {0}",

            [Keys.ItemScriptUpdater_Location_Invalid] = "Location in the file is invalid",

            [Keys.SceneScriptUpdater_SceneCountInvalid] = "Could not specify target scene. Please open only 1 scene.",
            [Keys.SceneScriptUpdater_Log_UpdateStart] = "Updating Scriptable Item Extensions...",
            [Keys.SceneScriptUpdater_Log_UpdateCompleted] = 
                "Scriptable Item Extension Updated: {0} scene objects, and {1} prefab objects",

            [Keys.VersionChecker_IsNewest_Title] = "CS Extensions: Current version is latest",
            [Keys.VersionChecker_IsNewest_CurrentVersion] = "Current version is the latest: {0}",
            [Keys.VersionChecker_IsNewest_CheckReleasePage] = "Check Release Page",
            [Keys.VersionChecker_NewVersionFound_Title] = "CS Extensions: New Version Detected",
            [Keys.VersionChecker_NewVersionFound_CurrentVersion] = "Current Version: {0}",
            [Keys.VersionChecker_NewVersionFound_LatestVersion] = "Latest Version:{0}",
            [Keys.VersionChecker_NewVersionFound_ReleaseNoteHeader] = "Latest Version Release Note:",
            [Keys.VersionChecker_NewVersionFound_OpenReleasePage] = "Open Release Page",
            [Keys.VersionChecker_Dialog_Close] = "Close",
            
            [Keys.VersionChecker_Failed_GetCurrentVersion] = "Failed to get current installed version",
            [Keys.VersionChecker_UPM_List_Check] = "Check Unity Package Manager list...",
            [Keys.VersionChecker_UPM_List_Check_Complete] = "Check Unity Package Manager list completed.",
            [Keys.VersionChecker_UPM_Check_Failed_Unknown] = "Failed to check current packages in project",
            [Keys.VersionChecker_UPM_Check_Failed_Parse] = "Failed to parse current package version",
            [Keys.VersionChecker_UPM_Check_Completed] = "UPM based CS Extensions version confirmed: {0}",
            [Keys.VersionChecker_UnityPackage_Failed_Parse] = "Failed to parse current version from package",
            [Keys.VersionChecker_UnityPackage_Completed] = ".unitypackage file based CS Extensions version confirmed: {0}",
            [Keys.VersionChecker_GetLatestVersion_Started] = "Check latest version of CS Extensions...",
            [Keys.VersionChecker_GetLatestVersion_Failed_Get] = "Failed to get latest release info from GitHub",
            [Keys.VersionChecker_GetLatestVersion_Failed_Parse] = "Failed to parse version number",
            [Keys.VersionChecker_GetLatestVersion_Completed] = "Latest version confirmed: {0}",
        };

        private static readonly Dictionary<Keys, string> JapaneseTexts = new()
        {
            [Keys.ScriptableItemExtension_Inspector_ReloadScript] = "Reload Script",
            [Keys.ScriptableItemExtension_Inspector_ResetValues] = "Reset Values",

            [Keys.ScriptExtensionField_UnsupportedType] = "(error: unsupported type!)",
            [Keys.ScriptExtensionField_HumanoidAnimation_NotHumanoid] = "Error: 指定したClipはHumanoidではありません。 Humanoid用のAnimationClipを指定してください。",
            [Keys.ScriptExtensionField_WorldItem_NotInScene] = "Error: シーン上のItemを指定してください",
            [Keys.ScriptExtensionField_WorldItemReferece_NotPrefab] = "Error: シーン上にないprefabを指定してください",
            [Keys.ScriptExtensionField_Material_IrrelatedToItem] = "Error: Itemかその子要素で使われているマテリアルを指定してください",
            
            [Keys.ScriptExtensionField_Warning_FailedToEstimateType] = "fieldの型が推定できませんでした: {0}",

            [Keys.ItemScriptUpdater_Location_Invalid] = "Location in the file is invalid",

            [Keys.SceneScriptUpdater_SceneCountInvalid] = "対象シーンが特定できませんでした。シーンを1つだけ開いている状態で再度試してください",
            [Keys.SceneScriptUpdater_Log_UpdateStart] = "Scriptable Item Extensions のスクリプトを再読み込み中...",
            [Keys.SceneScriptUpdater_Log_UpdateCompleted] = 
                "Scriptable Item Extension のスクリプトを再読み込みしました: シーン上の対象オブジェクト数:{0}, 対象prefab数: {1}",

            [Keys.VersionChecker_IsNewest_Title] = "CS Extensions: 現在のバージョンは最新です",
            [Keys.VersionChecker_IsNewest_CurrentVersion] = "現在のバージョン: {0}",
            [Keys.VersionChecker_IsNewest_CheckReleasePage] = "リリースページを確認",
            [Keys.VersionChecker_NewVersionFound_Title] = "CS Extensions: 新しいバージョンが見つかりました",
            [Keys.VersionChecker_NewVersionFound_CurrentVersion] = "現在のバージョン: {0}",
            [Keys.VersionChecker_NewVersionFound_LatestVersion] = "最新のバージョン:{0}",
            [Keys.VersionChecker_NewVersionFound_ReleaseNoteHeader] = "最新バージョンのリリースノート:",
            [Keys.VersionChecker_NewVersionFound_OpenReleasePage] = "リリースページを開く",
            [Keys.VersionChecker_Dialog_Close] = "Close",
            
            [Keys.VersionChecker_Failed_GetCurrentVersion] = "現在のインストールされているバージョンが特定できませんでした",
            [Keys.VersionChecker_UPM_List_Check] = "Unity Package Managerのインストール状況を確認...",
            [Keys.VersionChecker_UPM_List_Check_Complete] = "Unity Package Managerのインストール状況を正常に確認しました。",
            [Keys.VersionChecker_UPM_Check_Failed_Unknown] = "Unity Package Managerのインストール状況の確認に失敗しました。",
            [Keys.VersionChecker_UPM_Check_Failed_Parse] = "バージョン値が正常に読み取れませんでした。",
            [Keys.VersionChecker_UPM_Check_Completed] = "現在のバージョンをUnity Package Managerで確認しました: {0}",
            [Keys.VersionChecker_UnityPackage_Failed_Parse] = "package.jsonのバージョン値を正常に読み取れませんでした。",
            [Keys.VersionChecker_UnityPackage_Completed] = ".unitypackage でインストールした CS Extensions のバージョンを確認しました: {0}",
            [Keys.VersionChecker_GetLatestVersion_Started] = "最新の CS Extensions のバージョンを確認します...",
            [Keys.VersionChecker_GetLatestVersion_Failed_Get] = "GitHub で公開されている最新バージョンの取得に失敗しました。",
            [Keys.VersionChecker_GetLatestVersion_Failed_Parse] = "バージョン値が正常に読み取れませんでした。",
            [Keys.VersionChecker_GetLatestVersion_Completed] = "最新バージョンが確認できました: {0}",
        };

        public static string Get(Keys key)
        {
            var lang = LanguageChecker.Lang;
            if (lang == Language.Japanese)
            {
                var jp = JapaneseTexts.GetValueOrDefault(key, "");
                if (!string.IsNullOrEmpty(jp))
                {
                    return jp;
                }
            }

            return EnglishTexts[key];
        }
        
        public static string Get(Keys key, object value)
        {
            var rawText = Get(key);
            return string.Format(rawText, value);
        }

        public static string Get(Keys key, object value1, object value2)
        {
            var rawText = Get(key);
            return string.Format(rawText, value1, value2);
        }
    }
}
