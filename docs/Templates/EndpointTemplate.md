# Plantilla — Endpoint `METHOD /api/v1/<resource>`

- **Módulo:**
- **Permiso:** `Module.Action`
- **Descripción:**

## Request

```json
{ "FieldName": "value" }
```

## Response 200

```json
{
  "Success": true,
  "Message": "OK",
  "Data": {},
  "Errors": [],
  "TraceId": ""
}
```

## Errores

| Código HTTP | Error | Cuándo |
|---|---|---|
| 400 | `Module.InvalidRequest` | |
| 403 | `Module.Forbidden` | |

## Reglas de negocio aplicadas

1.

## Ejemplo de llamada

```bash
curl -X POST https://localhost:5001/api/v1/<resource> \
  -H "Authorization: Bearer <Token>" \
  -H "Content-Type: application/json" \
  -d '{ "FieldName": "value" }'
```
