#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# データ・シナリオの検証用のイメージ。従来の SRCDataLinter（lint）に加えて、srcsharp（自動プレイ・バランス集計など）を含む。
#   docker run --rm -v /path/to/data:/app/data koudenpa/srcdatalinter data              # 従来の lint
#   docker run --rm -v /path/to/scenario:/s koudenpa/srcdatalinter play /s/test.eve --script /s/test.play
# 使い方は docs/tools/authoring.md
# ビルドのコンテキストはリポジトリのルート（srcsharp help のヘルプ SRC.Sharp.Help/src を含めるため）。
#   docker build -f SRC.Sharp/SRCDataLinter/Dockerfile .
# コンテキストに含めるものは Dockerfile.dockerignore で絞る

# .NET 10 のイメージは Debian bookworm 版が無いため Ubuntu 24.04（noble）にする
FROM mcr.microsoft.com/dotnet/runtime:10.0-noble AS base
# シナリオを画面なしで動かす（SkiaSharp）ために fontconfig、日本語の描画に Noto Sans CJK（CI の VRT と同じ）
RUN apt-get update \
    && apt-get install -y --no-install-recommends libfontconfig1 fonts-noto-cjk \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /src
# バージョンは Directory.Build.props で決める
COPY ["SRC.Sharp/Directory.Build.props", "SRC.Sharp/"]
COPY ["SRC.Sharp/SRCDataLinter/SRCDataLinter.csproj", "SRC.Sharp/SRCDataLinter/"]
COPY ["SRC.Sharp/SRCSharpTools/SRCSharpTools.csproj", "SRC.Sharp/SRCSharpTools/"]
COPY ["SRC.Sharp/SRCCore/SRCCore.csproj", "SRC.Sharp/SRCCore/"]
COPY ["SRC.Sharp/SRCAuthoring/SRCAuthoring.csproj", "SRC.Sharp/SRCAuthoring/"]
COPY ["SRC.Sharp/SRCDebugAdapter/SRCDebugAdapter.csproj", "SRC.Sharp/SRCDebugAdapter/"]
COPY ["SRC.Sharp/SRCHelpSearch/SRCHelpSearch.csproj", "SRC.Sharp/SRCHelpSearch/"]
COPY ["SRC.Sharp/SRCPlay/SRCPlay.csproj", "SRC.Sharp/SRCPlay/"]
COPY ["SRC.Sharp/SRCRender/SRCRender.csproj", "SRC.Sharp/SRCRender/"]
RUN dotnet restore "SRC.Sharp/SRCDataLinter/SRCDataLinter.csproj" \
    && dotnet restore "SRC.Sharp/SRCSharpTools/SRCSharpTools.csproj"
COPY . .

FROM build AS publish
RUN dotnet publish "SRC.Sharp/SRCDataLinter/SRCDataLinter.csproj" -c Release -o /app/publish \
    && dotnet publish "SRC.Sharp/SRCSharpTools/SRCSharpTools.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["SRC.Sharp/SRCDataLinter/docker-entrypoint.sh", "/app/docker-entrypoint.sh"]
RUN chmod +x /app/docker-entrypoint.sh
ENTRYPOINT ["/app/docker-entrypoint.sh"]
