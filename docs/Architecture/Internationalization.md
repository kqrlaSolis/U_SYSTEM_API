# Internacionalización (i18n)

## Decisión

- **Español por defecto; inglés disponible.** Ambos idiomas se mantienen en paridad.
- Todos los textos visibles salen de un catálogo; **nada de textos hardcodeados** en el código.
- Los **identificadores siguen en inglés y PascalCase**; los valores traducidos son contenido, no código.
- Los **códigos de error no se traducen**; los mensajes sí.

Las claves son **PascalCase** y, en el caso de los errores, **coinciden con el código de error**
(`Users.DuplicateEmail`, `Validation.Required`). No se usa SCREAMING_SNAKE ni camelCase.

## API

### Contrato

- El cliente envía `Accept-Language: es-MX,es;q=0.9,en;q=0.8`.
- El servidor responde con `Message` y `ApiError.Message` en el idioma solicitado.
- `Code` es estable y en inglés: es lo que usa el cliente para decidir, no el mensaje.
- Si el idioma no está disponible, se usa `Localization.DefaultLanguage` y luego inglés.
- Si falta una clave, se registra una advertencia y se devuelve el código como mensaje.

```json
{
  "Success": false,
  "Message": "Ya existe un usuario con el correo ana@usystem.local.",
  "Data": null,
  "Errors": [
    {
      "Code": "Users.DuplicateEmail",
      "Field": "Email",
      "Message": "Ya existe un usuario con el correo ana@usystem.local."
    }
  ],
  "TraceId": "00-8f6c1e0f4f0d9a3b-1-00"
}
```

### Catálogos

| Archivo | Contenido |
|---|---|
| `Api/Resources/ErrorMessagesEs.json` | Mensajes en español |
| `Api/Resources/ErrorMessagesEn.json` | Mensajes en inglés |

Claves planas, en PascalCase, con el mismo nombre del código de error:

```json
{
  "Users.DuplicateEmail": "Ya existe un usuario con el correo {{Email}}.",
  "Grades.ReportClosed": "El acta está cerrada: se requiere reapertura.",
  "Validation.TooLong": "{{Field}} excede {{Max}} caracteres."
}
```

Placeholders entre `{{ }}` y en PascalCase (`{{Email}}`, `{{Max}}`). Nunca se concatenan mensajes: la
frase completa vive en el catálogo.

### Servicio

```csharp
using System.Globalization;

namespace USystem.Application.Localization;

public interface IErrorMessageProvider
{
    // Devuelve el mensaje traducido para un código estable
    string Get(string Code, string? Language, IReadOnlyDictionary<string, string>? Values = null);
}

public class ErrorMessageProvider : IErrorMessageProvider
{
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _MessagesByLanguage;

    public ErrorMessageProvider(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> MessagesByLanguage)
        => _MessagesByLanguage = MessagesByLanguage;

    public string Get(string Code, string? Language, IReadOnlyDictionary<string, string>? Values = null)
    {
        var LanguageCode = "es";
        if (!string.IsNullOrWhiteSpace(Language))
        {
            try
            {
                LanguageCode = CultureInfo.GetCultureInfo(Language).TwoLetterISOLanguageName;
            }
            catch (CultureNotFoundException)
            {
                // Idioma desconocido: se usa el predeterminado
                LanguageCode = "es";
            }
        }

        // Fallback: idioma solicitado, luego español, luego el código
        if (!_MessagesByLanguage.TryGetValue(LanguageCode, out var Catalog)
            && !_MessagesByLanguage.TryGetValue("es", out Catalog))
        {
            return Code;
        }

        if (!Catalog.TryGetValue(Code, out var Template))
        {
            return Code;
        }

        return Values is null ? Template : ReplacePlaceholders(Template, Values);
    }

    // Reemplaza {{Nombre}} por su valor
    private static string ReplacePlaceholders(string Template, IReadOnlyDictionary<string, string> Values)
        => Values.Aggregate(Template, (Current, Pair) => Current.Replace($"{{{{{Pair.Key}}}}}", Pair.Value));
}
```

Uso en un servicio:

```csharp
throw new BusinessRuleException(
    "Users.DuplicateEmail",
    new Dictionary<string, string> { ["Email"] = Request.Email });
```

El middleware de errores resuelve el mensaje con `IErrorMessageProvider` usando el idioma de la petición.
Los logs guardan el `Code` y los datos, nunca el mensaje traducido.

### Validación

FluentValidation devuelve códigos, no mensajes:

| Regla | Código |
|---|---|
| Campo obligatorio | `Validation.Required` |
| Longitud máxima | `Validation.TooLong` |
| Correo inválido | `Validation.InvalidEmail` |
| Rango de fechas | `Validation.InvalidRange` |
| CURP inválida | `Validation.InvalidCurp` |

El middleware los convierte a `ApiError` con `Field` y el mensaje localizado.

## Desktop

### Estructura

```
desktop/src/shared/i18n/
├── I18nProvider.tsx
├── Config.ts
├── Es.ts
└── En.ts
```

Se usa `react-i18next`, la misma librería que recomienda AXZY UI System para los textos de la aplicación.

### Catálogo

`Es.ts` es la fuente y `En.ts` debe cumplir el mismo tipo, de modo que el compilador exija paridad:

```ts
export const Es = {
  Common: {
    Save: 'Guardar',
    Cancel: 'Cancelar',
    Search: 'Buscar',
    Loading: 'Cargando…',
    NoResults: 'Sin resultados',
    Actions: 'Acciones',
  },
  Users: {
    List: {
      Title: 'Usuarios',
      New: 'Nuevo usuario',
      Empty: 'No hay usuarios registrados',
      Count: '{{Count}} usuarios',
    },
    Form: {
      TitleCreate: 'Nuevo usuario',
      TitleEdit: 'Editar usuario',
    },
    Columns: {
      Username: 'Usuario',
      Email: 'Correo',
      IsActive: 'Activo',
    },
  },
  Nav: {
    Students: 'Alumnos',
    Payments: 'Pagos',
  },
};

// El tipo exige las mismas claves en todos los idiomas
export type TranslationCatalog = typeof Es;
```

```ts
import type { TranslationCatalog } from './Es';

// El compilador falla si falta una clave o sobra una
export const En: TranslationCatalog = {
  Common: {
    Save: 'Save',
    Cancel: 'Cancel',
    Search: 'Search',
    Loading: 'Loading…',
    NoResults: 'No results',
    Actions: 'Actions',
  },
  Users: {
    List: {
      Title: 'Users',
      New: 'New user',
      Empty: 'No users found',
      Count: '{{Count}} users',
    },
    Form: {
      TitleCreate: 'New user',
      TitleEdit: 'Edit user',
    },
    Columns: {
      Username: 'Username',
      Email: 'Email',
      IsActive: 'Active',
    },
  },
  Nav: {
    Students: 'Students',
    Payments: 'Payments',
  },
};
```

### Uso

```tsx
import { useTranslation } from 'react-i18next';

export function UsersPageHeader() {
  const { t } = useTranslation();

  return <h1>{t('Users.List.Title')}</h1>;
}
```

Con valores:

```ts
t('Users.List.Count', { Count: Result.TotalCount }); // "{{Count}} usuarios"
```

Reglas:

- Las claves son PascalCase y con puntos (`Users.List.Title`).
- No se construyen claves con texto visible (`t('Error: ' + Message)` queda prohibido).
- El idioma se guarda en `localStorage` con la clave `USystem.Language` y se aplica al arrancar.
- El `HttpClient` envía `Accept-Language` con el idioma activo y actualiza la cabecera al cambiarlo.
- Los errores de la API se muestran con el `Message` que ya llega traducido; el código se usa solo para
  decidir el flujo.
- Fechas, números y moneda se formatean con `Intl` o con los helpers de `useITTranslation()`, nunca a mano.

### AXZY UI System

- La documentación completa de la librería está en `docs/AxzyUiSystem/` (63 componentes).
- AXZY UI tiene su propio catálogo interno: sin `ITI18nProvider` cae a español. Para que la librería
  siga el idioma de la app se monta `ITI18nProvider` con el locale activo.
- Las claves internas de AXZY (`common.save`, etc.) son de la librería y **no se renombran**: son la API
  de un tercero. Nuestro catálogo y el suyo conviven.
- Los componentes de `shared/components` son envoltorios con props en PascalCase que traducen a la API
  real de AXZY en un solo lugar.

## Añadir un texto

1. Agregar la clave en `Es.ts`.
2. Agregar la misma clave en `En.ts` (el tipo lo exige).
3. Usar `t('Namespace.Key')`; nada de español en el JSX.
4. Si es un error de API, agregar el código en `Rbac.py`/servicio, y la clave en ambos JSON de
   `Api/Resources/`.
5. Ejecutar las pruebas de paridad.

## Pruebas

- Test de paridad de claves entre `Es.ts` y `En.ts`.
- Test de paridad entre `ErrorMessagesEs.json` y `ErrorMessagesEn.json`.
- Test de la API con `Accept-Language: es` y `en` para un error de negocio.
- Test de fallback: idioma desconocido devuelve el idioma por defecto.
- Revisión visual: ninguna pantalla con texto hardcodeado.
