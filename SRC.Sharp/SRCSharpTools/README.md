# srcsharp

SRC# のデータ・シナリオの作成支援ツールです。設計は [docs/tools/authoring.md](https://github.com/7474/SRC/blob/master/docs/tools/authoring.md) を参照してください。

```sh
dotnet tool install -g SRCSharpTools
srcsharp lint path/to/scenario
srcsharp play path/to/scenario/test.eve --script test.play --out snapshots
srcsharp balance path/to/data --output balance.md
srcsharp help Askコマンド              # SRC# のヘルプ（リファレンス）を引く
srcsharp help --search 選択肢を表示する  # 関連の高い順にページを探す
srcsharp help --grep ChangeMap         # 文字列を含む行を探す
srcsharp help --export srcsharp-help   # ヘルプを Markdown で書き出す（AI に読ませるなど）
```

表示の言語は `--lang en` などか環境変数 `SRCSHARP_LANG` で切り替えます（[多言語対応](https://github.com/7474/SRC/blob/master/docs/i18n.md)）。

NuGet に無い版は、リポジトリから作って入れます。

```sh
dotnet pack SRC.Sharp/SRCSharpTools/SRCSharpTools.csproj -c Release -o out/srcsharp
dotnet tool install -g SRCSharpTools --add-source out/srcsharp
```

VS Code 拡張（`lsp`・`dap` を使う）の使い方は [SRCSharpVSCode の README](https://github.com/7474/SRC/blob/master/SRC.Sharp/SRCSharpVSCode/README.md) を参照してください。
