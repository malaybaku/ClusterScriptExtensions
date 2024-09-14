using System.Linq;
using ClusterVR.CreatorKit.Item.Implements;
using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.ComponentUpdater
{
    public static class HumanoidAnimationListUpdater
    {
        public static void Update(ScriptableItemExtension ext)
        {
            var elements = ext.ExtensionFields
                .Where(f => f.Type is ExtensionFieldType.HumanoidAnimation)
                .Select(f => (Id: f.FieldName, Clip: f.HumanoidAnimationClipValue))
                .ToArray();

            var component = ext.GetComponent<HumanoidAnimationList>();

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
                component = ext.gameObject.AddComponent<HumanoidAnimationList>();
            }

            //NOTE: animationプロパティを触りに行かないといけないので、一旦配列を作ったあとでSerializedObjectとして改修する
            var contents = elements
                .Select(elem => new HumanoidAnimationListEntry(elem.Id, null))
                .ToArray();
            component.Construct(contents);

            var so = new SerializedObject(component);
            var entriesProperty = so.FindProperty("humanoidAnimations");
            for (var i = 0; i < contents.Length; i++)
            {
                var entryProperty = entriesProperty.GetArrayElementAtIndex(i);
                entryProperty.FindPropertyRelative("animation").objectReferenceValue = elements[i].Clip;
            }
            so.ApplyModifiedProperties();
        }
    }    
}
