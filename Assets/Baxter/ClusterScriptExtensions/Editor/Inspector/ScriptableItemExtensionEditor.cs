using Baxter.ClusterScriptExtensions.Editor.ScriptUpdater;
using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.Inspector
{
    [CustomEditor(typeof(ScriptableItemExtension))]
    public class ScriptableItemExtensionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("templateCode"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                ReloadFields(serializedObject, true);
                Apply(serializedObject);
            }

            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            var fieldsProperty = serializedObject.FindProperty("extensionFields");
            var size = fieldsProperty.arraySize;
            for (var i = 0; i < size; i++)
            {
                EditorGUILayout.PropertyField(fieldsProperty.GetArrayElementAtIndex(i));
            }
            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                Apply(serializedObject);
            }

            if (GUILayout.Button("Reload Script"))
            {
                ReloadFields(serializedObject, false);
                Apply(serializedObject);
            }

            if (GUILayout.Button("Reset Values"))
            {
                // 値を巻き戻すのではなく、Field自体をリロードしてしまう。
                // こっちのほうがスクリプトが変わったケースに対して強いため
                ReloadFields(serializedObject, true);
                Apply(serializedObject);
            }
        }

        private static void ReloadFields(SerializedObject obj, bool refreshField)
        {
            var ext = (ScriptableItemExtension)obj.targetObject;
            FieldReloadUtil.ReloadFields(ext, refreshField);
            EditorUtility.SetDirty(ext);
        }

        private static void Apply(SerializedObject obj)
        {
            var ext = (ScriptableItemExtension)obj.targetObject;
            ItemScriptUpdater.ApplyGeneratedSourceCode(ext);
            // 何も呼ばないとScriptableItem側の表示が変わらなくて納得感がないため、再描画をリクエストする
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
    }
}
