using System.Linq;
using ClusterVR.CreatorKit.Item.Implements;
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

            EditorGUI.indentLevel++;

            switch (type)
            {
                case ExtensionFieldType.AudioClip:
                    ShowAdditionalContentForAudioClip(property, fieldName);
                    break;
                case ExtensionFieldType.HumanoidAnimation:
                    CheckErrorForHumanoidAnimation(valueProperty);
                    break;
                case ExtensionFieldType.WorldItem:
                    CheckWorldItemAssignValidity(valueProperty);
                    break;
                case ExtensionFieldType.WorldItemTemplate:
                    CheckWorldItemTemplateAssignValidity(valueProperty);
                    break;
                case ExtensionFieldType.Material:
                    CheckMaterialAssignValidity(valueProperty);
                    break;
            }

            EditorGUI.indentLevel--;
        }

        void ShowAdditionalContentForAudioClip(SerializedProperty property, string fieldName)
        {
            EditorGUILayout.PropertyField(
                property.FindPropertyRelative("boolValue"),
                new GUIContent(fieldName + ": loop")
            );
        }
        
        void CheckErrorForHumanoidAnimation(SerializedProperty valueProperty)
        {
            if (valueProperty.objectReferenceValue is AnimationClip clip && !clip.isHumanMotion)
            {
                ShowWarning("Error: The clip is not for Humanoid. Please set Humanoid animation.");
            }
        }

        void CheckWorldItemAssignValidity(SerializedProperty valueProperty)
        {
            var item = valueProperty.objectReferenceValue as Item;
            if (item == null)
            {
                return;
            }
            
            // - 永続化されたprefabそのものを指してたらNG
            // - prefab modeの場合にprefab mode内オブジェクトを指してるのはセーフ
            if (EditorUtility.IsPersistent(item.gameObject))
            {
                ShowWarning("Error: Please set item in scene.");
            }
        }

        void CheckWorldItemTemplateAssignValidity(SerializedProperty valueProperty)
        {
            var item = valueProperty.objectReferenceValue as Item;
            if (item == null)
            {
                return;
            }
            
            // - シーン上でprefabじゃないものを指してたらNG
            // - prefab modeの場合にprefab mode内オブジェクトを指すのもNG
            if (!EditorUtility.IsPersistent(item.gameObject))
            {
                ShowWarning("Error: Please set item prefab, which is not in scene.");
            }
        }
        
        void CheckMaterialAssignValidity(SerializedProperty valueProperty)
        {
            var material = valueProperty.objectReferenceValue as Material;
            if (material == null)
            {
                return;
            }

            // マテリアルはItemかその子要素で使っているものしか指定できない…という条件があるので、それを検証している
            var ext = (ScriptableItemExtension)valueProperty.serializedObject.targetObject;
            if (!ext.GetComponentsInChildren<Renderer>(true)
                .SelectMany(renderer => renderer.sharedMaterials)
                .Contains(material))
            {
                ShowWarning("Error: Please set material used by item or its children");
            }
        }
    }
}
