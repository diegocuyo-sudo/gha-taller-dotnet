namespace TareasApi;

public record Tarea(int Id, string Titulo, bool Hecha);

public record NuevaTarea(string? Titulo);

public record CambioEstado(bool Hecha);

// Estado en memoria: suficiente para el taller
public class TareasRepositorio
{
    private readonly Lock _candado = new();
    private readonly List<Tarea> _tareas = new()
    {
        new Tarea(1, "Crear el primer workflow", true),
        new Tarea(2, "Publicar la imagen en GHCR", false),
    };

    private int _siguienteId = 3;

    public IReadOnlyList<Tarea> Listar()
    {
        lock (_candado)
        {
            return _tareas.ToList();
        }
    }

    public Tarea Agregar(string titulo)
    {
        lock (_candado)
        {
            var tarea = new Tarea(_siguienteId++, titulo, false);
            _tareas.Add(tarea);
            return tarea;
        }
    }

    public Tarea? CambiarEstado(int id, bool hecha)
    {
        lock (_candado)
        {
            var indice = _tareas.FindIndex(t => t.Id == id);
            if (indice < 0)
            {
                return null;
            }

            var actualizada = _tareas[indice] with { Hecha = hecha };
            _tareas[indice] = actualizada;
            return actualizada;
        }
    }
}
