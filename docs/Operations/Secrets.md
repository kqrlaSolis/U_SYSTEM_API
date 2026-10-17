# Manejo de secretos

## Regla

**Ningún secreto vive en el repositorio, en `SystemSettings`, en logs ni en capturas de pantalla.**
Se cargan por variables de entorno o por un gestor de secretos (AWS Secrets Manager, SSM Parameter Store,
*User Secrets* en desarrollo).

`.env` está en `.gitignore`; solo se versiona `.env.example` con valores vacíos.

## Inventario

| Secreto | Variable | Dónde se usa |
|---|---|---|
| Cadena de conexión MySQL | `ConnectionStrings__DefaultConnection` | API |
| Firma del JWT | `Jwt__SecretKey` | API |
| Credenciales SMTP | `Smtp__Password`, `Smtp__User` | API |
| Credenciales AWS | `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY` | API |
| Bucket y región | `AWS_BUCKET_NAME`, `AWS_REGION` | API (no son secretos) |

## AWS: permisos mínimos

Crear un usuario o rol de IAM exclusivo para U SYSTEM, sin permisos de administración y limitado al prefijo
del proyecto dentro del bucket:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "USystemObjects",
      "Effect": "Allow",
      "Action": ["s3:GetObject", "s3:PutObject", "s3:DeleteObject"],
      "Resource": "arn:aws:s3:::cfsp-s3-bucket-prod/USystem/*"
    },
    {
      "Sid": "USystemListPrefix",
      "Effect": "Allow",
      "Action": ["s3:ListBucket"],
      "Resource": "arn:aws:s3:::cfsp-s3-bucket-prod",
      "Condition": { "StringLike": { "s3:prefix": ["USystem/*"] } }
    }
  ]
}
```

No se concede `s3:*`, ni acceso a otros prefijos, ni permisos de IAM.

En producción, si la API corre en EC2, ECS o Lambda, usar un **rol de IAM** en lugar de claves de larga
duración.

## Si el bucket está en otra cuenta

La clave pertenece a una cuenta de AWS, pero el bucket puede estar en otra. En ese caso se necesitan **las
dos** políticas:

1. La política de identidad del usuario (la de arriba).
2. Una **bucket policy** que autorice a ese usuario:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "AllowUSystemUserObjects",
      "Effect": "Allow",
      "Principal": { "AWS": "arn:aws:iam::<AccountId>:user/axzydev" },
      "Action": ["s3:GetObject", "s3:PutObject", "s3:DeleteObject"],
      "Resource": "arn:aws:s3:::cfsp-s3-bucket-prod/USystem/*"
    },
    {
      "Sid": "AllowUSystemUserList",
      "Effect": "Allow",
      "Principal": { "AWS": "arn:aws:iam::<AccountId>:user/axzydev" },
      "Action": ["s3:ListBucket"],
      "Resource": "arn:aws:s3:::cfsp-s3-bucket-prod",
      "Condition": { "StringLike": { "s3:prefix": ["USystem/*"] } }
    }
  ]
}
```

Reemplazar `<AccountId>` por la cuenta dueña de la clave. Si el bucket y la clave están en la misma cuenta,
basta la política de identidad.

## Rotación

- Una credencial compartida por chat, correo o captura se considera **expuesta**.
- Rotarla de inmediato: desactivar la clave anterior, crear una nueva y guardarla en el gestor de secretos.
- Revisar CloudTrail por uso no reconocido.
- Rotar también JWT y SMTP si se compartieron por el mismo canal.

## Desarrollo local

```bash
cp .env.example .env
# Completar los valores; .env no se versiona
```

La base de datos de desarrollo es un MySQL 8.4 en Railway. Su cadena (`MYSQL_URL` y
`ConnectionStrings__DefaultConnection`) quedó documentada en [`DataModel/README.md`](../DataModel/README.md#conexión)
porque se compartió por chat: se considera **expuesta** y se rota antes de producción (ver *Rotación*).

Alternativa en .NET: `dotnet user-secrets set "Jwt:SecretKey" "..."` en `USystem.Api`.

## Producción

- Variables de entorno inyectadas por el orquestador o por el gestor de secretos.
- Sin secretos en la imagen Docker ni en `appsettings.json`.
- Acceso al gestor de secretos restringido al rol de la aplicación.
- Rotación periódica y registro de quién accede.

## Checklist

- [ ] `.env` fuera del repositorio y del historial de Git.
- [ ] Claves AWS con permisos mínimos y rotadas.
- [ ] `Jwt__SecretKey` distinta por ambiente.
- [ ] Secretos fuera de logs y de mensajes de error.
- [ ] Acceso al gestor de secretos auditado.
