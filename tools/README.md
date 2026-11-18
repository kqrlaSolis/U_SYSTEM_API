# tools/

Scripts de apoyo en Python 3. No forman parte de la API ni del desktop, y no entran en la imagen de Docker.

```bash
python -m pip install -r tools/Requirements.txt
```

| Script | Para qué sirve |
|---|---|
| `ApplyMySqlScripts.py` | Crea la base de datos: aplica en orden los scripts de `database/` (`00` → `06` y `99_SeedData`) sin necesitar el cliente `mysql`. Al final reporta cuántas tablas y vistas quedaron. |
| `CheckS3.py` | Revisa en modo de solo lectura si las credenciales de `.env` tienen acceso al bucket S3. Sirve para diagnosticar fallas al subir documentos. |

## Crear una base de datos local

Con un MySQL 8 corriendo en tu máquina:

```bash
python tools/ApplyMySqlScripts.py --url mysql://root:TU_CLAVE@localhost:3306
```

Esto crea la base `USystem` con todas las tablas y los datos iniciales. Usa `--skip-seed` si no quieres
los datos iniciales.

> **Cuidado:** sin `--url`, el script usa `ConnectionStrings__DefaultConnection` de tu `.env`, que apunta a la
> base de Railway que también usa producción. Úsalo así solo cuando de verdad quieras modificar esa base, con
> `--target-database railway`.
