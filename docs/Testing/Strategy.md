# Estrategia de pruebas

| Nivel | Qué cubre | Herramienta sugerida |
|-------|-----------|----------------------|
| Unitarias (API) | Reglas de negocio en `Service`, con repositorios simulados | xUnit + Moq o NSubstitute |
| Integración (API) | Repositorios y endpoints contra MySQL real | xUnit + Testcontainers + `WebApplicationFactory` |
| Unitarias (Desktop) | ViewModels (hooks) y servicios | Vitest o Jest + React Testing Library (`renderHook`) |
| E2E | Flujos críticos en la app Electron | Playwright |
| Manuales / UAT | Validación con el personal de la escuela | `Templates/TestCaseTemplate.md` |

## Reglas

- Cada criterio de aceptación (`AcceptanceCriteria.md`) tiene al menos un caso de prueba.
- Las reglas de negocio de cada módulo se prueban en el `Service`.
- Los permisos se prueban en la API: sin permiso, 403.
- Datos de prueba ficticios; nunca datos reales de alumnos.
- Cobertura objetivo en servicios: ≥ 80 %.

## Internacionalización

- Test de paridad entre `ErrorMessagesEs.json` y `ErrorMessagesEn.json` (mismas claves).
- Test de paridad entre `Es.ts` y `En.ts` (el tipo lo exige en compilación, la prueba lo verifica en runtime).
- Test de la API con `Accept-Language: es` y `Accept-Language: en` para un error de negocio.
- Test de fallback: un idioma desconocido devuelve el idioma por defecto.

## Flujos críticos E2E

1. Login y bloqueo por intentos fallidos.
2. Alta de alumno, carga y validación de documentos.
3. Inscripción con requisitos incompletos (rechazo) y completa (éxito).
4. Reinscripción bloqueada por adeudo vencido, y con excepción autorizada.
5. Captura de calificaciones, cierre de acta e intento de modificación (rechazo).
6. Kardex y avance académico coherentes con las calificaciones.
7. Registro de pago con folio único y cancelación con motivo.

## Nomenclatura de pruebas

Los nombres de clases, métodos y variables de prueba también van en inglés y PascalCase:

```csharp
public class UserServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenEmailExists_ThrowsBusinessRuleException()
    {
        // Arrange
        var Request = new CreateUserRequest { Email = "ana@usystem.local" };

        // Act y Assert
        await Assert.ThrowsAsync<BusinessRuleException>(
            () => _UserService.CreateAsync(Request));
    }
}
```
