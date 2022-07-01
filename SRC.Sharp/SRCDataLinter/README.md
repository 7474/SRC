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


### Local build

```sh
dotnet run path/to/data
```
