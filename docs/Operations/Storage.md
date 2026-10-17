# Almacenamiento de archivos (S3)

## Decisión

Los documentos de alumnos y el material didáctico se guardan en un **bucket S3 compatible**
(AWS S3, MinIO o Backblaze). En la base de datos solo se guarda la referencia (`FilePath`) y los
metadatos; nunca el archivo.

## Contrato

```csharp
// Abstracción de almacenamiento; la implementación vive en Infrastructure
public interface IFileStorage
{
    Task<string> UploadAsync(
        string ObjectKey,
        Stream Content,
        string ContentType,
        CancellationToken Cancellation = default);

    Task<string> GetSignedUrlAsync(
        string ObjectKey,
        TimeSpan Lifetime,
        CancellationToken Cancellation = default);

    Task DeleteAsync(string ObjectKey, CancellationToken Cancellation = default);
}
```

Implementaciones: `S3FileStorage` (producción y desarrollo con MinIO) y `LocalFileStorage` (solo para
pruebas sin red). El contenedor de DI elige según `Storage.Provider`.

## Estructura del bucket

```
USystem/
├── Students/{StudentId}/Documents/{DocumentTypeId}/{FileName}
├── Students/{StudentId}/Photos/{FileName}
├── Subjects/{SubjectId}/Materials/{MaterialId}/{FileName}
└── Procedures/{ProcedureId}/{FileName}
```

Los nombres de objeto usan PascalCase en los segmentos fijos. El nombre original del archivo se
normaliza (sin acentos ni espacios) y se conserva en la base de datos si hace falta mostrarlo.

## Reglas

1. El bucket es **privado**; no hay lectura pública.
2. La descarga se hace con **URL firmada** de vida corta (5 minutos).
3. Cifrado en reposo con SSE-S3 (o el equivalente del proveedor).
4. Validación antes de subir: extensión en `Storage.AllowedExtensions` y tamaño máximo en
   `Storage.MaxFileSizeMb` (10 MB por defecto).
5. Tipo MIME permitido: PDF, JPG y PNG.
6. El archivo se sube primero y luego se confirma la transacción; si la transacción falla, se elimina el
   objeto.
7. No se guardan datos personales en el nombre del objeto ni en los metadatos del bucket.
8. Cada descarga de documento queda en `AuditLogs`.
9. Si una credencial se comparte por un canal inseguro (chat, correo, captura), se considera expuesta y se
   rota de inmediato (ver `Secrets.md`).

## Configuración

| Variable | Valor |
|---|---|
| `AWS_REGION` | `us-east-2` |
| `AWS_BUCKET_NAME` | `cfsp-s3-bucket-prod` |
| `AWS_ACCESS_KEY_ID` | Secreto: gestor de secretos o `.env` local |
| `AWS_SECRET_ACCESS_KEY` | Secreto: gestor de secretos o `.env` local |
| `AWS_ENDPOINT_URL` | Opcional; MinIO en desarrollo (`http://localhost:9000`) |

`Storage.Provider = S3` en `SystemSettings`. Las credenciales **no** se documentan aquí: ver
[`Secrets.md`](Secrets.md).

## Respaldos

- Activar **versionado** del bucket.
- Regla de ciclo de vida: conservar versiones 90 días; mover a almacenamiento frío a los 30 días.
- El respaldo de la base de datos y el del bucket deben coordinarse para que las referencias no queden
  huérfanas.
- Prueba de restauración: descargar un documento de prueba desde una versión anterior.

## Diagnóstico

| Síntoma | Causa probable | Solución |
|---|---|---|
| `403 Forbidden` en `head_bucket` pero STS responde | El usuario IAM no tiene permisos sobre el bucket | Adjuntar la política de identidad de `Secrets.md` |
| `403` en `list USystem/` | Falta `s3:ListBucket` | Agregar la acción con la condición del prefijo |
| `301` o `400` | Región incorrecta | Ajustar `AWS_REGION` (`us-east-2`) |
| `404` | El bucket no existe | Crear `cfsp-s3-bucket-prod` |
| `AccessDenied` al leer objetos | Falta `s3:GetObject` | Revisar la política o la bucket policy |

La respuesta de S3 incluye `x-amz-bucket-region`, que confirma la región real del bucket aunque el acceso
esté denegado.

Verificación de solo lectura:

```bash
python3 tools/CheckS3.py
```

El script imprime la identidad de la clave, la región del bucket, si puede listar `USystem/` y si puede leer
un objeto. No escribe nada ni muestra credenciales.

## Errores

| Código | Cuándo |
|---|---|
| `Storage.NotConfigured` | Faltan credenciales o bucket |
| `Storage.FileTypeNotAllowed` | Extensión fuera de la lista |
| `Storage.FileTooLarge` | Excede el máximo |
| `Storage.UploadFailed` | Error del proveedor |

Los mensajes están en `Api/Resources/ErrorMessagesEs.json` y `ErrorMessagesEn.json`.
