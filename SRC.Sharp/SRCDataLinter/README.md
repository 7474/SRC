# SRCDataLinter

SRC#のデータ・シナリオの解析CLIツールです。

## Usage

### GitHub Actions

See. https://github.com/7474/SRC-DataLinter

Example. https://github.com/7474/SRC-Data/blob/master/.github/workflows/validate-sharp-data.yml


### Docker

```sh
docker run --rm -v /path/to/data:/app/data -t koudenpa/srcdatalinter data
```

イメージには、データ・シナリオの検証全般を行う `srcsharp` も入っています。最初の引数が `srcsharp` のコマンド（`lint`・`play`・`balance` など）なら `srcsharp` を、それ以外は従来どおりこの lint を動かします。

```sh
# シナリオを画面なしで自動プレイする（.play スクリプトの手順で進め、スナップショットを保存する）
docker run --rm -v /path/to/scenario:/s koudenpa/srcdatalinter play /s/test.eve --script /s/test.play --out /s/snapshots
# ユニットデータの能力値の一覧と外れ値
docker run --rm -v /path/to/data:/d koudenpa/srcdatalinter balance /d
# SARIF で lint（GitHub のコードスキャンに渡せる）
docker run --rm -v /path/to/data:/d koudenpa/srcdatalinter lint /d --format sarif --output /d/lint.sarif
```

詳しくは [作成支援ツール](../../docs/tools/authoring.md) を参照してください。


### Local build

```sh
dotnet run path/to/data
```
