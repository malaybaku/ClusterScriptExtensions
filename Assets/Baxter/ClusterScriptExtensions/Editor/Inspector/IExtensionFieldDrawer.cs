using UnityEditor;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Editor.Inspector
{
    public interface IExtensionFieldDrawer
    {
        /// <summary>
        /// この値が大きいほど、優先的に<see cref="CanDraw"/>や<see cref="Draw"/>が呼ばれる。
        /// 特殊な条件のプロパティ描画を行いたいI/F実装では大きめの値を返し、
        /// フォールバック的な汎用の描画を行うI/F実装では小さめの値を返す。
        /// </summary>
        int Priority { get; }
        
        /// <summary>
        /// 指定したプロパティをこのI/F実装が描画できるかどうかを判定する。
        /// trueを返した場合、<see cref="GetPropertyHeight"/>と<see cref="Draw"/>が呼ばれる
        /// </summary>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CanDraw(SerializedProperty property, ExtensionFieldType type);

        /// <summary>
        /// 指定したプロパティの描画高さを取得する。
        /// 呼び出し元は <see cref="CanDraw"/> trueになるようなプロパティでのみ、このメソッドを呼ぶ
        /// </summary>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        float GetPropertyHeight(SerializedProperty property, ExtensionFieldType type);

        /// <summary>
        /// 指定したプロパティを描画する。
        /// 呼び出し元は <see cref="CanDraw"/> trueになるようなプロパティでのみ、このメソッドを呼ぶ
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        void Draw(Rect position, SerializedProperty property, ExtensionFieldType type);
    }
}
