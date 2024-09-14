using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.Inspector.ExtensionFieldDrawer
{
    using static FieldDrawerUtils;

    /// <summary>
    /// 数値などの基本的なプリミティブに対し、オーバーライドの有無と値のカスタムUIの2つを提示する実装
    /// </summary>
    public class OverridablePrimitiveDrawer : IExtensionFieldDrawer
    {
        public int Priority => 0;
        
        public bool CanDraw(SerializedProperty property, ExtensionFieldType type)
        {
            //NOTE: 暗黙に、このプロパティより
            return (int)type < (int)ExtensionFieldType.AssetReferenceIndexOffset;
        }

        public float GetPropertyHeight(SerializedProperty property, ExtensionFieldType type)
        {
            return LineHeight(2f);
        }

        public void Draw(Rect position, SerializedProperty property, ExtensionFieldType type)
        {
            ShowOverrideBoolValueProperty(position, property);
            
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
                // CanDrawで弾いてるから通常は来ないはず
                EditorGUILayout.LabelField("(error: unsupported type!)");
                return;
            }

            EditorGUI.BeginDisabledGroup(!IsOverrideActive(property));

            position.y += LineHeight(1f);
            position.height = LineHeight(1f);
            
            var fieldName = GetFieldName(property);
            var fieldLabel = new GUIContent(fieldName);
            
            // Sliderを表示する場合もUIの高さは据え置きなので、だいたい同等に扱う
            var hasRange = property.FindPropertyRelative("hasRange").boolValue;
            if (type is ExtensionFieldType.Int && hasRange)
            {
                EditorGUI.IntSlider(
                    position,
                    valueProperty,
                    (int)property.FindPropertyRelative("rangeMin").floatValue,
                    (int)property.FindPropertyRelative("rangeMax").floatValue,
                    fieldLabel
                );
            }
            else if (type is ExtensionFieldType.Float && hasRange)
            {
                EditorGUI.Slider(
                    position,
                    valueProperty,
                    property.FindPropertyRelative("rangeMin").floatValue,
                    property.FindPropertyRelative("rangeMax").floatValue,
                    fieldLabel
                );
            }
            else
            {
                EditorGUI.PropertyField(position, valueProperty, fieldLabel);
            }
            
            EditorGUI.EndDisabledGroup();
        }
    }
}

