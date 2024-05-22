FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Play.Frontend/src/Play.Frontend/Play.Frontend.csproj", "Play.Frontend/src/Play.Frontend/"]
RUN dotnet restore "Play.Frontend/src/Play.Frontend/Play.Frontend.csproj"
COPY . .
WORKDIR "/src/Play.Frontend/src/Play.Frontend"
RUN dotnet build "Play.Frontend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Play.Frontend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Play.Frontend.dll"]
