using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.Inspector.ExtensionFieldDrawer
{
    using static FieldDrawerUtils;

    /// <summary>
    /// AnimationClipなどのアセット参照を表示する実装。
    /// 個別コンポーネントのエラーチェックなども含んでいるので、
    /// 分岐を細分化したい場合はそれをアセットの種類別に細分化してもよい
    /// </summary>
    public class AssetReferenceDrawer : IExtensionFieldDrawer
    {
        public int Priority => 1;

        public bool CanDraw(SerializedProperty property, ExtensionFieldType type)
        {
            return (int)type >= (int)ExtensionFieldType.AssetReferenceIndexOffset;
        }

        public float GetPropertyHeight(SerializedProperty property, ExtensionFieldType type)
        {
            return LineHeight(1f);
        }

        public void Draw(Rect position, SerializedProperty property, ExtensionFieldType type)
        {
            var valueProperty = type switch
            {
                ExtensionFieldType.AudioClip => property.FindPropertyRelative("audioClipValue"),
                ExtensionFieldType.HumanoidAnimation => property.FindPropertyRelative("humanoidAnimationClipValue"),
                ExtensionFieldType.WorldItem => property.FindPropertyRelative("itemReferenceValue"),
                ExtensionFieldType.WorldItemTemplate => property.FindPropertyRelative("itemReferenceValue"),
                ExtensionFieldType.Material => property.FindPropertyRelative("materialValue"),
                _ => null,
            };

            if (valueProperty == null)
            {
                // CanDrawで弾いてるから通常は来ないはず
                EditorGUILayout.LabelField("(error: unsupported type!)");
                return;
            }
            
            position.height = LineHeight(1f);
            
            var fieldName = GetFieldName(property);
            EditorGUI.PropertyField(position, valueProperty, new GUIContent(fieldName));

            // AudioClipでは他と違ってループの有無も設定が必要なので、それを表示する
            if (type is ExtensionFieldType.AudioClip)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(
                    property.FindPropertyRelative("boolValue"),
                    new GUIContent(fieldName + ": loop")
                );
                EditorGUI.indentLevel--;
            }
            
            if (type is ExtensionFieldType.HumanoidAnimation)
            {
                CheckErrorForHumanoidAnimation(valueProperty);
            }
        }

        void CheckErrorForHumanoidAnimation(SerializedProperty valueProperty)
        {
            if (valueProperty.objectReferenceValue is AnimationClip clip && !clip.isHumanMotion)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(
                    "Error: The clip seems not Humanoid Animation!", 
                    (GUIStyle) "CN StatusWarn"
                );
                EditorGUI.indentLevel--;
            }
        }
    }
}
