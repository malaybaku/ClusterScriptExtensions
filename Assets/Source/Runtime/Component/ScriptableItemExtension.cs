using ClusterVR.CreatorKit.Item.Implements;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions
{
    /// <summary>
    /// Scriptable Itemを拡張してSerializeField的な表示ができるようにするクラス。
    /// 実体はTemplate CodeとTemplate Codeから一定のルールで抽出されたフィールド情報の一覧になっている
    /// </summary>
    [RequireComponent(typeof(ScriptableItem))]
    public class ScriptableItemExtension : MonoBehaviour
    {
        [SerializeField] private JavaScriptAsset templateCode;
        [SerializeField] private ScriptExtensionField[] extensionFields;

        public JavaScriptAsset TemplateCode => templateCode;
        public ScriptExtensionField[] ExtensionFields => extensionFields;

        public void SetFields(ScriptExtensionField[] fields) => extensionFields = fields;
    }
}
