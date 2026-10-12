# gha-taller-dotnet · API de tareas en ASP.NET Core para aprender GitHub Actions

App de ejemplo del **Taller de GitHub Actions para C#**.

- `src/TareasApi/` API minimal de ASP.NET Core (.NET 10): `/health`, `/api/version`, `/api/tareas` y una página en `wwwroot/`
- `tests/TareasApi.Tests/` 7 tests con xUnit y `WebApplicationFactory`
- `Dockerfile` imagen multi-stage (SDK para compilar, runtime de ASP.NET para correr)
- `global.json` fija la versión del SDK (.NET 10)
- `soluciones/` los workflows terminados de cada paso del taller

## Correr en local (requiere el SDK de .NET 10)

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/TareasApi     # http://localhost:5000
```

## Workflows del taller (copiar a `.github/workflows/`)

| Paso | Archivo | Qué hace |
| --- | --- | --- |
| 1 | `hola.yml` | Primer workflow: SDKs de .NET en la máquina de GitHub |
| 2 | `ci.yml` | Formato, build y tests en Linux y Windows, reporte de tests y cobertura |
| 3 | `docker.yml` | Build, smoke test y push de la imagen a `ghcr.io` |
| 4 | `deploy-render.yml` + `docker-con-render.yml` | Despliegue reutilizable en Render y `docker.yml` con el job `deploy` |
| 5 | `release.yml` | En cada tag `vX.Y.Z`: CI, imagen semver, deploy con aprobación y Release con el zip publicado |
| 6 | `dependabot.yml` | Va en `.github/dependabot.yml` (NuGet, actions y Docker) |
