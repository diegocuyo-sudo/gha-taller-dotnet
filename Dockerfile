# ---- Etapa 1: compilar y publicar con el SDK ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Primero solo el .csproj: la restauración queda en caché mientras no cambien las dependencias
COPY src/TareasApi/TareasApi.csproj src/TareasApi/
RUN dotnet restore src/TareasApi/TareasApi.csproj

COPY src/ src/
RUN dotnet publish src/TareasApi/TareasApi.csproj -c Release -o /app --no-restore

# ---- Etapa 2: imagen final solo con el runtime de ASP.NET ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Datos de versión inyectados por el pipeline con --build-arg
ARG APP_VERSION=dev
ARG GIT_SHA=local
ENV APP_VERSION=$APP_VERSION \
    GIT_SHA=$GIT_SHA \
    ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app .

USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "TareasApi.dll"]
