# Checklist de seguridad previo a producción

- [ ] `Jwt:SecretKey` fuerte y fuera del repositorio.
- [ ] Usuario de base de datos de aplicación sin privilegios DDL.
- [ ] HTTPS configurado y CORS restringido al origen del desktop.
- [ ] Contraseña del administrador inicial cambiada.
- [ ] Swagger deshabilitado o protegido en producción.
- [ ] Logs sin contraseñas, tokens ni datos sensibles.
- [ ] Respaldos probados y cifrados.
- [ ] Bucket S3 privado, con versionado y cifrado en reposo.
- [ ] Credenciales AWS rotadas, con permisos mínimos y fuera de Git.
- [ ] URLs firmadas de vida corta; sin objetos públicos.
- [ ] Permisos revisados con la escuela (`Security/RbacMatrix.md`).
- [ ] Dependencias auditadas: `dotnet list package --vulnerable` y `npm audit`.
- [ ] Instaladores de Windows, Linux y macOS firmados.
- [ ] `AuditLogs` sin endpoints de modificación ni borrado.
- [ ] `.editorconfig` y ESLint aplicados en CI (nomenclatura en inglés y PascalCase).
- [ ] Catálogos i18n con paridad es/en (API y desktop).
