FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5078

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PlatformWellDataSync.csproj", "./"]
RUN dotnet restore "PlatformWellDataSync.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "PlatformWellDataSync.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlatformWellDataSync.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlatformWellDataSync.dll"]
