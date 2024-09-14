using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.Inspector
{
    /// <summary>
    /// <see cref="ScriptableItemExtension"/>のフィールド値のプロパティ描画するときに使うサブルーチン。
    /// </summary>
    public static class FieldDrawerUtils
    {
        /// <summary>
        /// インスペクターの1行あたりの高さに倍率をかけた高さを取得する。
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static float LineHeight(float factor)
            => EditorGUIUtility.singleLineHeight * factor;
        
        /// <summary>
        /// フィールド名を取得する。
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetFieldName(SerializedProperty property)
            => property.FindPropertyRelative("fieldName").stringValue;

        /// <summary>
        /// overrideの有無をプロパティとして表示する。
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        public static void ShowOverrideBoolValueProperty(Rect position, SerializedProperty property)
        {
            var fieldName = GetFieldName(property);
            var pos = position;
            pos.height = LineHeight(1f);
            var prop = property.FindPropertyRelative("overrideValue");
            EditorGUI.PropertyField(pos, prop, new GUIContent(fieldName + ": override"));
        }
        
        public static bool IsOverrideActive(SerializedProperty property)
            => property.FindPropertyRelative("overrideValue").boolValue;
    }
}
