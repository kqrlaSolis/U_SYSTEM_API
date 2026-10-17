// Almacenamiento de archivos con contrato S3: AWS S3 en producción, MinIO en desarrollo.
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Infrastructure.Storage;

public class S3FileStorage : IFileStorage, IDisposable
{
    private readonly IAmazonS3 _Client;
    private readonly StorageOptions _Options;

    public S3FileStorage(IOptions<StorageOptions> Options)
    {
        _Options = Options.Value;

        var ClientConfiguration = new AmazonS3Config();
        if (!string.IsNullOrWhiteSpace(_Options.ServiceUrl))
        {
            // MinIO y proveedores compatibles no usan virtual-host style
            ClientConfiguration.ServiceURL = _Options.ServiceUrl;
            ClientConfiguration.ForcePathStyle = true;
            ClientConfiguration.AuthenticationRegion = _Options.Region;
        }
        else
        {
            ClientConfiguration.RegionEndpoint = RegionEndpoint.GetBySystemName(_Options.Region);
        }

        // Las credenciales salen de la cadena estándar de AWS (variables de entorno o rol de IAM)
        _Client = new AmazonS3Client(ClientConfiguration);
    }

    public async Task<string> UploadAsync(
        string Key,
        Stream Content,
        string ContentType,
        CancellationToken Cancellation = default)
    {
        var FullKey = BuildKey(Key);
        var Request = new PutObjectRequest
        {
            BucketName = EnsureConfigured(),
            Key = FullKey,
            InputStream = Content,
            ContentType = ContentType,
            AutoCloseStream = false,
        };

        await _Client.PutObjectAsync(Request, Cancellation);
        return FullKey;
    }

    public async Task<Stream> DownloadAsync(string Key, CancellationToken Cancellation = default)
    {
        var Response = await _Client.GetObjectAsync(EnsureConfigured(), BuildKey(Key), Cancellation);

        // Se copia a memoria: el llamador no debe depender del ciclo de vida de la respuesta HTTP
        var Buffer = new MemoryStream();
        await using (Response.ResponseStream)
        {
            await Response.ResponseStream.CopyToAsync(Buffer, Cancellation);
        }

        Buffer.Position = 0;
        return Buffer;
    }

    public async Task DeleteAsync(string Key, CancellationToken Cancellation = default)
        => await _Client.DeleteObjectAsync(EnsureConfigured(), BuildKey(Key), Cancellation);

    public Task<string> GetPresignedUrlAsync(
        string Key,
        TimeSpan? Expiration = null,
        CancellationToken Cancellation = default)
    {
        var Minutes = _Options.PresignedUrlMinutes <= 0 ? 15 : _Options.PresignedUrlMinutes;
        var Request = new GetPreSignedUrlRequest
        {
            BucketName = EnsureConfigured(),
            Key = BuildKey(Key),
            Expires = DateTime.UtcNow.Add(Expiration ?? TimeSpan.FromMinutes(Minutes)),
            Verb = HttpVerb.GET,
        };

        return Task.FromResult(_Client.GetPreSignedURL(Request));
    }

    public async Task<bool> ExistsAsync(string Key, CancellationToken Cancellation = default)
    {
        try
        {
            await _Client.GetObjectMetadataAsync(EnsureConfigured(), BuildKey(Key), Cancellation);
            return true;
        }
        catch (AmazonS3Exception CaughtException) when (CaughtException.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private string EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_Options.BucketName))
        {
            throw new BusinessRuleException("Storage.NotConfigured");
        }

        return _Options.BucketName;
    }

    // Todos los archivos del sistema viven bajo el prefijo del proyecto dentro del bucket
    private string BuildKey(string Key)
    {
        var CleanKey = Key.TrimStart('/');
        return string.IsNullOrWhiteSpace(_Options.Prefix) ? CleanKey : $"{_Options.Prefix.TrimEnd('/')}/{CleanKey}";
    }

    public void Dispose() => _Client.Dispose();
}
