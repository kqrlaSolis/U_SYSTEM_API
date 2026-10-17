// Opciones del almacenamiento de archivos (sección "Storage" o variables Storage__*).
namespace USystem.Infrastructure.Storage;

public class StorageOptions
{
    public const string SectionName = "Storage";

    // Local o S3 (S3 cubre AWS, MinIO y Backblaze)
    public string Provider { get; set; } = "S3";

    public string BucketName { get; set; } = string.Empty;

    public string Region { get; set; } = "us-east-2";

    // Solo para MinIO u otro proveedor compatible; vacío en AWS
    public string ServiceUrl { get; set; } = string.Empty;

    // Prefijo del proyecto dentro del bucket; evita compartir espacio con otros sistemas
    public string Prefix { get; set; } = "USystem";

    public int PresignedUrlMinutes { get; set; } = 15;

    public int MaxFileSizeMb { get; set; } = 10;

    public string AllowedExtensions { get; set; } = "pdf,jpg,jpeg,png";
}
