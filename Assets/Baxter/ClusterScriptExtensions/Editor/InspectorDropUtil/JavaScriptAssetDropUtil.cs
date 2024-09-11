using Baxter.ClusterScriptExtensions.Editor.ScriptUpdater;
using ClusterVR.CreatorKit.Item.Implements;
using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor
{
    public static class JavaScriptAssetInspectorDropUtil
    {
        [InitializeOnLoadMethod]
        public static void InitializeHandler()
        {
            DragAndDrop.AddDropHandler(InspectorDropHandler);
        }

        private static DragAndDropVisualMode InspectorDropHandler(Object[] targets, bool perform)
        {
            if (targets.Length != 1 || targets[0] is not GameObject gameObject)
            {
                return DragAndDropVisualMode.None;
            }

            var objects = DragAndDrop.objectReferences;
            if (objects.Length != 1)
            {
                return DragAndDropVisualMode.None;
            }

            var obj = objects[0];
            if (obj is not JavaScriptAsset jsAsset)
            {
                return DragAndDropVisualMode.None;
            }

            // Drop前: Dropの許可までで終わり
            if (!perform)
            {
                return DragAndDropVisualMode.Copy;
            }
            
            // Drop実行: JavaScriptAssetのドロップをScriptable Itemの追加と同等と見なす
            var ext = gameObject.GetComponent<ScriptableItemExtension>();
            if (ext == null)
            {
                ext = gameObject.AddComponent<ScriptableItemExtension>();
            }

            if (ext.TemplateCode != jsAsset)
            {
                ext.TemplateCode = jsAsset;
                FieldReloadUtil.ReloadFields(ext, true);
                ItemScriptUpdater.ApplyGeneratedSourceCode(ext);
                EditorUtility.SetDirty(ext.gameObject);
            }

            return DragAndDropVisualMode.Copy;
        }
    }
}