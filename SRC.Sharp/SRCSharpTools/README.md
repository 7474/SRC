# srcsharp

SRC# のデータ・シナリオの作成支援ツールです。設計は [docs/tools/authoring.md](https://github.com/7474/SRC/blob/master/docs/tools/authoring.md) を参照してください。

```sh
dotnet tool install -g SRCSharpTools
srcsharp lint path/to/scenario
srcsharp play path/to/scenario/test.eve --script test.play --out snapshots
srcsharp balance path/to/data --output balance.md
```
