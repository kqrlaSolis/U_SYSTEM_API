// Carga el archivo .env del repositorio en las variables de entorno del proceso.
// Formato admitido: KEY=VALUE, comentarios con #, líneas vacías y comillas opcionales.
namespace USystem.Api.Configuration;

public static class EnvironmentFileLoader
{
    // Sube por el árbol de directorios desde el binario hasta encontrar el .env del repositorio
    public static string? Load(string? StartDirectory = null, int MaxLevels = 8)
    {
        var CurrentDirectory = new DirectoryInfo(StartDirectory ?? AppContext.BaseDirectory);

        for (var Level = 0; Level < MaxLevels && CurrentDirectory is not null; Level++)
        {
            var Candidate = Path.Combine(CurrentDirectory.FullName, ".env");
            if (File.Exists(Candidate))
            {
                LoadFile(Candidate);
                return Candidate;
            }

            CurrentDirectory = CurrentDirectory.Parent;
        }

        return null;
    }

    public static void LoadFile(string FilePath)
    {
        foreach (var Line in File.ReadAllLines(FilePath))
        {
            var TrimmedLine = Line.Trim();

            if (TrimmedLine.Length == 0 || TrimmedLine.StartsWith('#'))
            {
                continue;
            }

            var SeparatorIndex = TrimmedLine.IndexOf('=');
            if (SeparatorIndex <= 0)
            {
                continue;
            }

            var Key = TrimmedLine[..SeparatorIndex].Trim();
            var Value = TrimmedLine[(SeparatorIndex + 1)..].Trim();

            // Se aceptan valores entre comillas dobles o simples
            if (Value.Length >= 2
                && ((Value[0] == '"' && Value[^1] == '"') || (Value[0] == '\'' && Value[^1] == '\'')))
            {
                Value = Value[1..^1];
            }

            // No se sobreescribe lo que ya definió el sistema operativo o el orquestador
            if (Environment.GetEnvironmentVariable(Key) is null)
            {
                Environment.SetEnvironmentVariable(Key, Value);
            }
        }
    }
}
