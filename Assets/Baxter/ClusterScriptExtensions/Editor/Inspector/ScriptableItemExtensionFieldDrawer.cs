using System.Linq;
using Baxter.ClusterScriptExtensions.Editor.Inspector.ExtensionFieldDrawer;
using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.Inspector
{
    [CustomPropertyDrawer(typeof(ScriptExtensionField))]
    public class ScriptExtensionFieldDrawer : PropertyDrawer
    {
        static ScriptExtensionFieldDrawer()
        {
            var drawers = new IExtensionFieldDrawer[]
            {
                new TextAreaStringDrawer(),
                new AssetReferenceDrawer(),
                new OverridablePrimitiveDrawer(),
            };

            Drawers = drawers.OrderByDescending(d => d.Priority).ToArray();
        }

        // NOTE: 
        private static readonly IExtensionFieldDrawer[] Drawers;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var fieldType = GetFieldType(property);
            foreach (var drawer in Drawers)
            {
                if (drawer.CanDraw(property, fieldType))
                {
                    return drawer.GetPropertyHeight(property, fieldType);
                }
            }

            // NOTE: エラー表示はEditorGUILayoutで行うため、高さを確保する必要がない
            return 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var fieldType = GetFieldType(property);
            foreach (var drawer in Drawers)
            {
                if (drawer.CanDraw(property, fieldType))
                {
                    drawer.Draw(position, property, fieldType);
                    return;
                }
            }
            
            EditorGUILayout.LabelField("(error: unsupported type!)");
        }

        private static ExtensionFieldType GetFieldType(SerializedProperty property)
            => (ExtensionFieldType)property.FindPropertyRelative("type").intValue;
    }
}
