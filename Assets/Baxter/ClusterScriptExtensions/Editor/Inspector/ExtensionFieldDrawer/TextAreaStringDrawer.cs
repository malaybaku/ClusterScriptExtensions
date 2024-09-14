using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.Inspector.ExtensionFieldDrawer
{
    using static FieldDrawerUtils;

    /// <summary>
    /// 複数行を表示して編集できるような文字列プロパティを提示する
    /// </summary>
    public class TextAreaStringDrawer : IExtensionFieldDrawer
    {
        private const float HeightFactor = 2f;
        private const float ScrollAreaHeightFactor = 3.5f;
        private Vector2 scrollPosition = Vector2.zero;

        public int Priority => 10;

        public bool CanDraw(SerializedProperty property, ExtensionFieldType type)
        {
            return type is ExtensionFieldType.String &&
                property.FindPropertyRelative("useTextArea").boolValue;
        }

        public float GetPropertyHeight(SerializedProperty property, ExtensionFieldType type)
        {
            return LineHeight(HeightFactor);
        }

        public void Draw(Rect position, SerializedProperty property, ExtensionFieldType type)
        {
            // 見え方としては overrideの有無、フィールド名、TextAreaフィールドがこの順に表示される
            //   foo: override
            //   foo
            //   [    ]
            //   [    ]
            //   [    ]
            
            ShowOverrideBoolValueProperty(position, property);

            var singleLineHeight = LineHeight(1f);
            position.y += singleLineHeight;
            position.height = singleLineHeight;
            var fieldName = GetFieldName(property);
            EditorGUI.LabelField(position, fieldName);

            EditorGUI.BeginDisabledGroup(!IsOverrideActive(property));
            
            //NOTE: ラベルとテキストエリアの間の隙間が広すぎるのを調整している
            //EditorGUILayout.Space(-10);
            scrollPosition = EditorGUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.Height(LineHeight(ScrollAreaHeightFactor))
            );

            var valueProperty = property.FindPropertyRelative("stringValue");
            valueProperty.stringValue = EditorGUILayout.TextArea(
                valueProperty.stringValue,
                GUILayout.ExpandHeight(true)
            );
            EditorGUILayout.EndScrollView();
            
            EditorGUI.EndDisabledGroup();
        }
    }
}
