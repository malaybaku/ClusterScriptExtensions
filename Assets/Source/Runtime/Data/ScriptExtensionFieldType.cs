namespace Baxter.ClusterScriptExtensions
{
    /// <summary>
    /// Fieldの種類の一覧
    /// </summary>
    public enum ExtensionFieldType
    {
        // プリミティブ的であって参照構造がないものを定義している
        Bool = 0,
        Int,
        Float,
        String,
        Vector2,
        Vector3,
        Quaternion,

        // もしアセット参照にかかわるField (WorldItemReferenceなど) を追加する場合、
        // オフセットをつけて分割する: プリミティブと混ざるとややこしいため
        //AssetReferenceIndexOffset = 1000,
    }
}
