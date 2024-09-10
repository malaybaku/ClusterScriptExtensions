# ClusterScript Extensions

ClusterScript Extensions (CS Extensions) は [Cluster Script](https://docs.cluster.mu/script/) の機能を拡張するプロジェクトです。

MonoBehaviour で記述したスクリプトに近い編集体験を提供することを目指しています。

## Install

ご使用のプロジェクトに [Cluster Creator Kit](https://docs.cluster.mu/creatorkit/) のv2.20.0またはそれ以降が導入されていることを確認します。

その後、 [Releases](https://github.com/malaybaku/ClusterScriptExtensions/releases) に含まれている `.unitypackage` ファイルをダウンロードし、プロジェクトに導入します。


## Usage

### `Scriptable Item Extension` の使用

CS Extensionsの機能を使うには、`Scriptable Item`の代わりに`Scriptable Item Extension` コンポーネントをアタッチします。

`Scriptable Item Extension` をアタッチしたオブジェクトでは `Scriptable Item` の内容が自動で更新するため、 `Scriptable Item` は直接編集しないようにします。 `Source Code Asset` も空のままにしておきます。

(TODO: 初期設定がわかるスクリーンショット)

同一のプロジェクトやワールド上で、 `Scriptable Item Extension` を導入したゲームオブジェクトと、導入していないゲームオブジェクトが混在していても問題ありません。

`Scriptable Item Extension` をアタッチしたのち、 `Template Code` にスクリプトを指定します。

以上の一連の操作を行う代わりに、 GameObject のインスペクターに直接 `.js` 拡張子のあるファイルをドラッグ & ドロップすることでも `Scriptable Item Extension` コンポーネントを追加できます。

(TODO: ドラッグドロップのbefore/afterがわかるスクリーンショット)

### スクリプトの適用とフィールド編集

`Template Code` に指定したスクリプト内で `// @field` から始まるコメントを記述すると、その次の行に定義した変数は Unity Editor のインスペクターから編集できるようになります。編集可能なフィールドは `const` として定義しておきます。

```javascript
// @field(string)
const myValue = "example";

$.onStart(() => {
  $.log(`Hello, ${myValue}!`);
});
```

上記のスクリプトを `Template Code` に指定すると、インスペクター上では以下のように表示されます。

(TODO: 初期設定がわかるスクリーンショット)

プロパティの `override` にチェックを入れて編集することで、実際に値を上書きできます。
編集結果は直ちに `Scriptable Item` に適用されます。

現時点では下記のような書き方で、7種類のデータ型をサポートしています。

```
// @field(bool)
const myBool = false;

// @field(int)
const myInt = 1;

// @field(float)
const myFloat = 2;

// @field(string)
const myString = "Test";

// @field(Vector2)
const myVector2 = new Vector2(1, 2);

// @field(Vector3)
const myVector3 = new Vector3(3, 4, 5);

// @field(Quaternion)
const myQuaternion = new Quaternion();
```

`Quaternion` 以外では、初期値を直接的な数値として記載してあればインスペクター上でも初期値として反映されます。
`1 + 2` など、数値そのものではない表記を行うと、初期値は0や空文字などのデフォルト値として扱われます。

また、上記の `// @field` から始まるコメントは関数内に記述すると動作しません。例えば、 `$.onStart()` の内側で上記のコメントを記述しても無視されます。


### Template Codeに差分が発生したときの操作

`Template Code` に指定したスクリプトを書き換えたがフィールド値の再編集は行わない場合、ワールドのアップロード前にスクリプトの差分を適用する必要があります。

メニューバーで `CS Extensions` > `Apply Template Codes used in Scene` を選択すると、現在開いているシーン上から参照されている `Scriptable Item Extension` を一括でチェックし、必要に応じてスクリプトの内容が更新されます。

(TODO: メニューバーのスクリーンショット)

この更新操作は、 [Create Item Gimmick](https://docs.cluster.mu/creatorkit/gimmick-components/create-item-gimmick/) や [World Item Template List](https://docs.cluster.mu/creatorkit/item-components/world-item-template-list/) で指定された Prefab も対象として動作します。


## 内部挙動とデバッグについて

`Scriptable Item Extension` を起点とするエディタ拡張では下記の処理を行います。

- `Template Code` のスクリプトを構文解析し、書き換え可能なフィールドを検出します。
- `Template Code` の構文解析の結果、およびインスペクターで指定されたフィールド値を組み合わせることでスクリプトの文字列を新たに生成し、 `Scriptable Item` に適用します。

適用結果は通常の `Scriptable Item` のスクリプトと同等に扱われます。

もし必要な場合、生成された `Scriptable Item` のテキストをコピーしてテキストエディタにペーストするなどの方法によって、CS Extensionsが期待通りに動作しているかどうかを確認できます。


## License

レポジトリ上でソースコードとして含まれるコードのライセンスは[LICENSE](./LICENSE)に基づきます。

## Third Party

本レポジトリに含まれる `Esprima.dll` は [esprima-dotnet](https://github.com/sebastienros/esprima-dotnet) の配布バイナリです。ライセンスは [LICENSE.txt](./Assets/Source/Editor/DLLs/LICENSE.txt) を参照して下さい。
