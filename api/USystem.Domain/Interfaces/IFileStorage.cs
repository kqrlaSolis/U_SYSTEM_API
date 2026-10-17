// Almacenamiento de archivos con contrato S3 (AWS S3, MinIO o Backblaze).
namespace USystem.Domain.Interfaces;

public interface IFileStorage
{
    Task<string> UploadAsync(
        string Key,
        Stream Content,
        string ContentType,
        CancellationToken Cancellation = default);

    Task<Stream> DownloadAsync(string Key, CancellationToken Cancellation = default);

    Task DeleteAsync(string Key, CancellationToken Cancellation = default);

    Task<string> GetPresignedUrlAsync(
        string Key,
        TimeSpan? Expiration = null,
        CancellationToken Cancellation = default);

    Task<bool> ExistsAsync(string Key, CancellationToken Cancellation = default);
}
