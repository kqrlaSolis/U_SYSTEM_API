# Flujo de Git

- `main`: estable, solo versiones liberadas.
- `develop`: integración.
- Ramas de trabajo desde `develop`:
  - `feature/<Module>-<ShortName>` → `feature/Users-PasswordReset`
  - `fix/<Module>-<ShortName>` → `fix/Grades-ReportClose`
- Pull Request obligatorio con revisión; debe incluir pruebas y actualizar la documentación del módulo.
- Etiquetas de versión: `v<Major>.<Minor>.<Patch>`.
- Los scripts SQL nuevos se agregan como scripts numerados; **nunca** se edita un script ya ejecutado en
  producción (se agrega uno nuevo).

## Mensajes de commit

Formato: `<Type>(<Module>): <Description>` con la descripción en inglés.

| Tipo | Uso |
|---|---|
| `feat` | Nueva funcionalidad |
| `fix` | Corrección de error |
| `refactor` | Cambio sin alterar comportamiento |
| `docs` | Documentación |
| `test` | Pruebas |
| `chore` | Tareas de mantenimiento |

Ejemplos:

```
feat(Users): add password reset flow
fix(Grades): block capture on closed academic term
docs(DataModel): document StudentDocuments unique key
```

## Reglas de revisión

- [ ] Todo el código nuevo está en inglés y PascalCase.
- [ ] Los comentarios están en español.
- [ ] Los permisos nuevos existen en `99_SeedData.sql` y en la matriz RBAC.
- [ ] Hay pruebas para las reglas de negocio nuevas.
