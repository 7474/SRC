#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS build
WORKDIR /src
COPY ["SRCDataLinter/SRCDataLinter.csproj", "SRCDataLinter/"]
COPY ["SRCCore/SRCCore.csproj", "SRCCore/"]
RUN dotnet restore "SRCDataLinter/SRCDataLinter.csproj"
COPY . .
WORKDIR "/src/SRCDataLinter"
RUN dotnet build "SRCDataLinter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SRCDataLinter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "/app/SRCDataLinter.dll"]