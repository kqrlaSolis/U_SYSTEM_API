// Fact que solo corre contra la base de datos real cuando USYSTEM_RUN_DB_TESTS=1.
// Sin la bandera la prueba aparece como "Skipped" en lugar de pasar sin probar nada.
namespace USystem.Tests.Support;

public sealed class DatabaseFactAttribute : FactAttribute
{
    public const string EnableVariable = "USYSTEM_RUN_DB_TESTS";

    public DatabaseFactAttribute()
    {
        if (Environment.GetEnvironmentVariable(EnableVariable) != "1")
        {
            Skip = $"Prueba contra la base de datos real: define {EnableVariable}=1 para ejecutarla.";
        }
    }
}
