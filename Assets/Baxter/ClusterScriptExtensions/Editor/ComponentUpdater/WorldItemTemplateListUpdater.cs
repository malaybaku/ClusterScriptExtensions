using System.Linq;
using ClusterVR.CreatorKit.Item.Implements;
using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.ComponentUpdater
{
    public static class WorldItemTemplateListUpdater
    {
        public static void Update(ScriptableItemExtension ext)
        {
            var entries = ext.ExtensionFields
                .Where(f => f.Type is ExtensionFieldType.WorldItemTemplate && f.ItemReferenceValue != null)
                .Select(f => (Id: f.FieldName, Item: f.ItemReferenceValue))
                .ToArray();

            var component = ext.GetComponent<WorldItemTemplateList>();

            // コンポーネントが要らない場合、削除した状態を正とする
            if (entries.Length == 0)
            {
                if (component != null)
                {
                    Object.DestroyImmediate(component);
                    EditorUtility.SetDirty(ext.gameObject);
                }
                return;
            }

            // 必要なら普通に追加
            if (component == null)
            {
                component = ext.gameObject.AddComponent<WorldItemTemplateList>();
            }

            // public propertyがないので、シリアライズした内容ベースでリストを構築する
            var so = new SerializedObject(component);
            var entriesProperty = so.FindProperty("worldItemTemplates");
            entriesProperty.ClearArray();
            for (var i = 0; i < entries.Length; i++)
            {
                entriesProperty.InsertArrayElementAtIndex(i);
                var elem = entriesProperty.GetArrayElementAtIndex(i);
                elem.FindPropertyRelative("id").stringValue = entries[i].Id;
                elem.FindPropertyRelative("worldItemTemplate").objectReferenceValue = entries[i].Item;
            }
            so.ApplyModifiedProperties();
        }
    }
}
