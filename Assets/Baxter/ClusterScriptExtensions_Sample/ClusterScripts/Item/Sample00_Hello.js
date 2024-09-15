// 文字列 string をカスタムできる、もっとも基本的なサンプル。

// @field(string)
const displayName = "Alice";

$.onInteract(() => { 
  showLog(`Sample00:\nHello, ${displayName}!`);
});

// $.log だとサンプルシーン上で目視できないので空間上のテキストに表示している。
// 自作シーンでチェックする場合は単に $.log(text) で十分。これ以降のサンプルも同様
const showLog = (text) => { 
  $.subNode("LogText").setText(text);
  $.log(text);
}
