using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.Inspector
{
    [CustomPropertyDrawer(typeof(ScriptExtensionField))]
    public class ScriptExtensionFieldDrawer : PropertyDrawer
    {
        private const float TextAreaTotalLineHeightFactor = 6f;
        private const float LineHeightFactor = 2.5f;
        private const float TextAreaLineHeightFactor = TextAreaTotalLineHeightFactor - LineHeightFactor;

        private Vector2 textAreaScrollPosition;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (UseTextArea(property))
            {
                return LineHeight(LineHeightFactor);
            }
            else
            {
                return LineHeight(LineHeightFactor);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 「overrideするかどうか + overrideする対象の値」みたいな見え方にする。
            // foo: override   [x]
            // foo             [  42  ]
            var fieldName = property.FindPropertyRelative("fieldName").stringValue;

            var overrideValuePos = position;
            overrideValuePos.height = LineHeight(1f);
            var overrideValueProperty = property.FindPropertyRelative("overrideValue");
            EditorGUI.PropertyField(overrideValuePos, overrideValueProperty, new GUIContent(fieldName + ": override"));

            var useTextArea = UseTextArea(property);
            
            var propertyPosition = position;
            propertyPosition.y = position.y + LineHeight(1f);
            propertyPosition.height = LineHeight(1f);

            // textAreaを使う場合、propertyPositionの位置にはラベルだけ書いて下にずらす
            if (useTextArea)
            {
                EditorGUI.LabelField(propertyPosition, fieldName);
                propertyPosition.y += LineHeight(1f);
                propertyPosition.height = LineHeight(TextAreaLineHeightFactor);
            }
            
            var type = (ExtensionFieldType)property.FindPropertyRelative("type").intValue;
            var valueProperty = type switch
            {
                ExtensionFieldType.Bool => property.FindPropertyRelative("boolValue"),
                ExtensionFieldType.Int => property.FindPropertyRelative("intValue"),
                ExtensionFieldType.Float => property.FindPropertyRelative("floatValue"),
                ExtensionFieldType.String => property.FindPropertyRelative("stringValue"),
                ExtensionFieldType.Vector2 => property.FindPropertyRelative("vector2Value"),
                ExtensionFieldType.Vector3 => property.FindPropertyRelative("vector3Value"),
                ExtensionFieldType.Quaternion => property.FindPropertyRelative("quaternionValue"),
                _ => null,
            };

            if (valueProperty == null)
            {
                EditorGUILayout.LabelField("(error: unsupported type!)");
                return;
            }
            
            EditorGUI.BeginDisabledGroup(!overrideValueProperty.boolValue);

            var fieldLabel = new GUIContent(fieldName);

            if (type is ExtensionFieldType.Int &&
                property.FindPropertyRelative("hasRange").boolValue)
            {
                EditorGUI.IntSlider(
                    propertyPosition,
                    valueProperty,
                    (int)property.FindPropertyRelative("rangeMin").floatValue,
                    (int)property.FindPropertyRelative("rangeMax").floatValue,
                    fieldLabel
                );
            }
            else if (
                type is ExtensionFieldType.Float &&
                property.FindPropertyRelative("hasRange").boolValue)
            {
                EditorGUI.Slider(
                    propertyPosition,
                    valueProperty,
                    property.FindPropertyRelative("rangeMin").floatValue,
                    property.FindPropertyRelative("rangeMax").floatValue,
                    fieldLabel
                );
            }
            else if (useTextArea)
            {
                //NOTE: ラベルとテキストエリアの間の隙間が広すぎるのを調整している
                EditorGUILayout.Space(-10);
                textAreaScrollPosition = EditorGUILayout.BeginScrollView(
                    textAreaScrollPosition,
                    GUILayout.Height(propertyPosition.height)
                );
                valueProperty.stringValue = EditorGUILayout.TextArea(
                    valueProperty.stringValue,
                    GUILayout.ExpandHeight(true)
                );
                EditorGUILayout.EndScrollView();
                EditorGUILayout.Space();
            }
            else
            {
                EditorGUI.PropertyField(propertyPosition, valueProperty, fieldLabel);
            }
            
            EditorGUI.EndDisabledGroup();
        }

        private static bool UseTextArea(SerializedProperty property)
        {
            var type = (ExtensionFieldType)property.FindPropertyRelative("type").intValue;
            return
                type is ExtensionFieldType.String &&
                property.FindPropertyRelative("useTextArea").boolValue;
        }
        
        private static float LineHeight(float factor) => EditorGUIUtility.singleLineHeight * factor;
    }
}
