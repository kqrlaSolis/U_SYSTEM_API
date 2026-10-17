# Datos personales

U SYSTEM maneja datos personales y sensibles de alumnos (CURP, domicilio, documentos oficiales y
situación económica).

## Acceso

- Restringido por permisos: `Students.View`, `StudentDocuments.View`, `StudentDocuments.Validate`.
- Los documentos descargados quedan registrados en la bitácora.
- El personal de finanzas ve el estado de cuenta, no los documentos.

## Almacenamiento

- Los archivos viven en un bucket S3 privado; en `StudentDocuments` solo se guarda `FilePath`.
- La descarga se hace con URL firmada de vida corta (5 minutos); no hay objetos públicos.
- Los respaldos se cifran y se almacenan fuera del servidor de base de datos.
- No se registran datos sensibles en logs.

## Retención y baja

- La retención y la baja de datos se definen con la escuela conforme al aviso de privacidad y a la
  legislación local de protección de datos.
- Los expedientes de alumnos egresados conservan su historial académico; los documentos se conservan
  según la normativa institucional.

## Pruebas y capacitación

- Usar datos ficticios o anonimizados; nunca datos reales de alumnos.
- Las capturas de pantalla para capacitación no deben contener datos personales reales.

> Este documento es una guía técnica, no asesoría legal.
