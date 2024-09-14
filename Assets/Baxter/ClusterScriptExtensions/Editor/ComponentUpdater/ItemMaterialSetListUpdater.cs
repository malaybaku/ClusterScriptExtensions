using System.Linq;
using ClusterVR.CreatorKit.Item;
using ClusterVR.CreatorKit.Item.Implements;
using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.ComponentUpdater
{
    public static class ItemMaterialSetListUpdater
    {
        public static void Update(ScriptableItemExtension ext)
        {
            var elements = ext.ExtensionFields
                .Where(f => f.Type is ExtensionFieldType.Material && f.MaterialValue != null)
                .Select(f => (Id: f.FieldName, Material: f.MaterialValue))
                .ToArray();

            var component = ext.GetComponent<ItemMaterialSetList>();

            // コンポーネントが要らない場合、削除した状態を正とする
            if (elements.Length == 0)
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
                component = ext.gameObject.AddComponent<ItemMaterialSetList>();
            }

            var contents = elements
                .Select(elem => new ItemMaterialSet(elem.Id, elem.Material))
                .ToArray();
            component.Construct(contents);
        }
    }    
}
