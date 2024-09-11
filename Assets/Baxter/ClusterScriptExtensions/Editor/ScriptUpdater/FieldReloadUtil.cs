using System;
using System.Linq;
using Baxter.ClusterScriptExtensions.Editor.ScriptParser;

namespace Baxter.ClusterScriptExtensions.Editor.ScriptUpdater
{
    public static class FieldReloadUtil
    {
        /// <summary>
        /// スクリプトの内容に即してField一覧を更新する。
        /// Template Codeが変化した場合はrefresh = trueにして呼び出すことで、
        /// 新Scriptに誤って値を引き継ぐのを防止する。
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="refresh"></param>
        public static void ReloadFields(ScriptableItemExtension ext, bool refresh)
        {
            var templateCode = ext.TemplateCode?.text;
            if (string.IsNullOrEmpty(templateCode))
            {
                ext.SetFields(Array.Empty<ScriptExtensionField>());
            }
            else
            {
                var fields = ExtensionFieldParser.ExtractTargetFields(templateCode);
                foreach (var f in fields)
                {
                    InitializeExtensionFieldValue(f, ext.ExtensionFields, refresh);
                }
                ext.SetFields(fields);
            }
        }
        
        private static void InitializeExtensionFieldValue(
            ScriptExtensionField field, ScriptExtensionField[] existingFields, bool refresh)
        {
            if (refresh)
            {
                field.ResetValues();
                return;
            }

            // - 名前と型が同じフィールドの値があれば持ち越す
            // - そうでない場合、スクリプトを参考に初期値が定まる
            var existingField = existingFields.FirstOrDefault(
                ef => ef.FieldName == field.FieldName && ef.Type == field.Type
            );
            
            if (existingField != null)
            {
                field.CopyValues(existingField);
            }
            else
            {
                field.ResetValues();
            }
        }
    }
}
