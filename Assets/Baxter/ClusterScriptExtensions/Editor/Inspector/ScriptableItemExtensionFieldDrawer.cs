using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.Inspector
{
    [CustomPropertyDrawer(typeof(ScriptExtensionField))]
    public class ScriptExtensionFieldDrawer : PropertyDrawer
    {
        private const float LineHeightFactor = 2.5f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
            => LineHeight(LineHeightFactor);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 「overrideするかどうか + overrideする対象の値」みたいな見え方にする。
            // foo: override   [x]
            // foo             [  42  ]
            var fieldName = property.FindPropertyRelative("fieldName").stringValue;

            var overrideValuePos = position;
            overrideValuePos.y = position.y - LineHeight(0.25f);
            overrideValuePos.height = LineHeight(1f);
            var overrideValueProperty = property.FindPropertyRelative("overrideValue");
            EditorGUI.PropertyField(overrideValuePos, overrideValueProperty, new GUIContent(fieldName + ": override"));

            var propertyPosition = position;
            propertyPosition.y = position.y + LineHeight(0.75f);
            propertyPosition.height = LineHeight(1f);
            
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

            if (valueProperty != null)
            {
                EditorGUI.BeginDisabledGroup(!overrideValueProperty.boolValue);
                EditorGUI.PropertyField(propertyPosition, valueProperty, new GUIContent(fieldName));
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.LabelField("(error: unsupported type!)");
            }
        }
        
        private static float LineHeight(float factor) => EditorGUIUtility.singleLineHeight * factor;
    }
}
