using System.Collections.Generic;
using System.Linq;
using ClusterVR.CreatorKit.Gimmick.Implements;
using ClusterVR.CreatorKit.Item.Implements;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Baxter.ClusterScriptExtensions.Editor.ScriptUpdater
{
    public static class SceneScriptUpdater
    {
        /// <summary>
        /// 現在のシーンで使われている <see cref="ScriptableItemExtension"/> に対し、
        /// 現在のソースコードとフィールド一覧の内容に基づいてスクリプトを再生成する。
        /// ワールドのアップロード前に呼び出すのが想定する使い方。
        /// </summary>
        [MenuItem("Baxter/Apply Scriptable Item Extension Field Values")]
        public static void UpdateClusterScriptsInCurrentScene()
        {
            if (EditorSceneManager.loadedSceneCount is 0 or > 1)
            {
                Debug.LogError("Could not specify target scene. Please open only 1 scene.");
                return;
            }

            var scene = SceneManager.GetActiveScene();
            UpdateClusterScripts(scene);
        }

        
        /// <summary>
        /// この処理でやること
        /// - 以下の3箇所から、ワールドで使われうるScriptable Item Extensionを取得
        ///   1. 指定したシーンに直接あるやつ
        ///   2. シーン上のWorld Item Template Listで参照されてるやつ
        ///   3. シーン上のCreate Item Gimmickで参照されてるやつ
        ///   (※そのうちExtension自身が参照したobjも含める必要がありそうだが、まだ無いので不要)
        /// - 見つけたExtensionについて、3,2,1の順で下記を行ってScriptable Itemの内容をリフレッシュする
        ///   - フィールドの再チェック
        ///   - コードを生成してScriptable Itemへ書き込む
        /// - 1の書き込みはシーン上に対してであり、prefabに対しては書き込まない
        /// - 2と3はprefabへ書き込む
        /// </summary>
        /// <param name="scene"></param>
        private static void UpdateClusterScripts(Scene scene)
        {
            var extensionsInScene = GetAllComponentsInScene<ScriptableItemExtension>(scene);
            var extensionsByWorldItemTemplateList = GetAllComponentsInScene<WorldItemTemplateList>(scene)
                .SelectMany(list => list.ItemTemplates())
                .SelectMany(template => template.Item.gameObject
                    .GetComponentsInChildren<ScriptableItemExtension>(true)
                );
            var extensionsByCreateItemGimmick = GetAllComponentsInScene<CreateItemGimmick>(scene)
                .SelectMany(gimmick => gimmick.ItemTemplate.gameObject
                    .GetComponentsInChildren<ScriptableItemExtension>(true)
                );

            // prefabの更新
            foreach (var ext in
                extensionsByCreateItemGimmick.Concat(extensionsByWorldItemTemplateList))
            {
                //NOTE: ext自体がprefabに属しているはずだが、念のためやっている
                var prefabExt = PrefabUtility.GetCorrespondingObjectFromSource(ext);
                FieldReloadUtil.ReloadFields(prefabExt, false);
                ScriptCodeGenerator.ApplyGeneratedSourceCode(prefabExt);
                var obj = prefabExt.gameObject;
                PrefabUtility.SaveAsPrefabAsset(obj, AssetDatabase.GetAssetPath(obj));
            }
            
            // シーン本体を更新
            foreach (var ext in extensionsInScene)
            {
                FieldReloadUtil.ReloadFields(ext, false);
                ScriptCodeGenerator.ApplyGeneratedSourceCode(ext);
                PrefabUtility.RecordPrefabInstancePropertyModifications(ext);
            }
        }

        private static IEnumerable<T> GetAllComponentsInScene<T>(Scene scene)
        {
            return scene
                .GetRootGameObjects()
                .SelectMany(obj => obj.GetComponentsInChildren<T>(true));
        }
    }
}
