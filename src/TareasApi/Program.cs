using TareasApi;

var builder = WebApplication.CreateBuilder(args);

// Render (y otros PaaS) indican el puerto en la variable PORT
var puerto = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(puerto))
{
    builder.WebHost.UseUrls($"http://*:{puerto}");
}

builder.Services.AddSingleton<TareasRepositorio>();

var app = builder.Build();

// Sirve la página de wwwroot/index.html en la raíz
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/health", () => Results.Ok(new { status = "ko" }));

app.MapGet("/api/version", (IConfiguration config) =>
{
    var version = config["APP_VERSION"] ?? "dev";
    var commit = config["GIT_SHA"] ?? "local";
    return Results.Ok(new { version, commit });
});

app.MapGet("/api/tareas", (TareasRepositorio repo) => Results.Ok(repo.Listar()));

app.MapPost("/api/tareas", (NuevaTarea nueva, TareasRepositorio repo) =>
{
    if (string.IsNullOrWhiteSpace(nueva.Titulo))
    {
        return Results.BadRequest(new { error = "El campo \"titulo\" es obligatorio" });
    }

    var tarea = repo.Agregar(nueva.Titulo.Trim());
    return Results.Created($"/api/tareas/{tarea.Id}", tarea);
});

app.MapPatch("/api/tareas/{id:int}", (int id, CambioEstado cambio, TareasRepositorio repo) =>
{
    var tarea = repo.CambiarEstado(id, cambio.Hecha);
    if (tarea is null)
    {
        return Results.NotFound(new { error = "Tarea no encontrada" });
    }

    return Results.Ok(tarea);
});

app.Run();

// Permite que los tests levanten la API con WebApplicationFactory<Program>
public partial class Program
{
}
