#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# データ・シナリオの検証用のイメージ。従来の SRCDataLinter（lint）に加えて、srcsharp（自動プレイ・バランス集計など）を含む。
#   docker run --rm -v /path/to/data:/app/data koudenpa/srcdatalinter data              # 従来の lint
#   docker run --rm -v /path/to/scenario:/s koudenpa/srcdatalinter play /s/test.eve --script /s/test.play
# 使い方は docs/tools/authoring.md

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
COPY ["Directory.Build.props", "./"]
COPY ["SRCDataLinter/SRCDataLinter.csproj", "SRCDataLinter/"]
COPY ["SRCSharpTools/SRCSharpTools.csproj", "SRCSharpTools/"]
COPY ["SRCCore/SRCCore.csproj", "SRCCore/"]
COPY ["SRCAuthoring/SRCAuthoring.csproj", "SRCAuthoring/"]
COPY ["SRCDebugAdapter/SRCDebugAdapter.csproj", "SRCDebugAdapter/"]
COPY ["SRCPlay/SRCPlay.csproj", "SRCPlay/"]
COPY ["SRCRender/SRCRender.csproj", "SRCRender/"]
RUN dotnet restore "SRCDataLinter/SRCDataLinter.csproj" \
    && dotnet restore "SRCSharpTools/SRCSharpTools.csproj"
COPY . .

FROM build AS publish
RUN dotnet publish "SRCDataLinter/SRCDataLinter.csproj" -c Release -o /app/publish \
    && dotnet publish "SRCSharpTools/SRCSharpTools.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["SRCDataLinter/docker-entrypoint.sh", "/app/docker-entrypoint.sh"]
RUN chmod +x /app/docker-entrypoint.sh
ENTRYPOINT ["/app/docker-entrypoint.sh"]
