using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace TareasApi.Tests;

// xUnit crea una instancia nueva por test: cada test levanta su propia API limpia
public sealed class ApiTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _fabrica = new();

    public void Dispose() => _fabrica.Dispose();

    [Fact]
    public async Task Health_responde_ok()
    {
        var cliente = _fabrica.CreateClient();

        var respuesta = await cliente.GetFromJsonAsync<EstadoSalud>("/health");

        Assert.Equal("ok", respuesta?.Status);
    }

    [Fact]
    public async Task Version_usa_la_configuracion()
    {
        var fabrica = _fabrica.WithWebHostBuilder(b =>
        {
            b.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?> { ["APP_VERSION"] = "9.9.9" });
            });
        });
        var cliente = fabrica.CreateClient();

        var respuesta = await cliente.GetFromJsonAsync<InfoVersion>("/api/version");

        Assert.Equal("9.9.9", respuesta?.Version);
    }

    [Fact]
    public async Task Listar_devuelve_las_tareas_iniciales()
    {
        var cliente = _fabrica.CreateClient();

        var tareas = await cliente.GetFromJsonAsync<List<Tarea>>("/api/tareas");

        Assert.Equal(2, tareas?.Count);
    }

    [Fact]
    public async Task Crear_devuelve_201_con_la_tarea()
    {
        var cliente = _fabrica.CreateClient();

        var respuesta = await cliente.PostAsJsonAsync("/api/tareas", new { titulo = "Desplegar a Render" });
        var tarea = await respuesta.Content.ReadFromJsonAsync<Tarea>();

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.Equal(new Tarea(3, "Desplegar a Render", false), tarea);
    }

    [Fact]
    public async Task Crear_sin_titulo_devuelve_400()
    {
        var cliente = _fabrica.CreateClient();

        var respuesta = await cliente.PostAsJsonAsync("/api/tareas", new { titulo = "" });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task Marcar_como_hecha_actualiza_la_tarea()
    {
        var cliente = _fabrica.CreateClient();

        var respuesta = await cliente.PatchAsJsonAsync("/api/tareas/2", new { hecha = true });
        var tarea = await respuesta.Content.ReadFromJsonAsync<Tarea>();

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.True(tarea?.Hecha);
    }

    [Fact]
    public async Task Marcar_tarea_inexistente_devuelve_404()
    {
        var cliente = _fabrica.CreateClient();

        var respuesta = await cliente.PatchAsJsonAsync("/api/tareas/999", new { hecha = true });

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    private sealed record EstadoSalud(string Status);

    private sealed record InfoVersion(string Version, string Commit);
}
