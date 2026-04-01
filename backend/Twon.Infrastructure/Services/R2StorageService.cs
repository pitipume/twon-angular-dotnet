using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Twon.Application.Common.Interfaces;

namespace Twon.Infrastructure.Services;

public class R2StorageService(IAmazonS3 s3, IConfiguration config) : IStorageService
{
    private readonly string _bucket = config["R2:BucketName"] ?? "twon";

    public async Task UploadAsync(string key, Stream content, string contentType)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            InputStream = content,
            ContentType = contentType,
            DisablePayloadSigning = true  // required for R2
        };
        await s3.PutObjectAsync(request);
    }

    public async Task<string> GetSignedReadUrlAsync(string key, int expirySeconds)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucket,
            Key = key,
            Expires = DateTime.UtcNow.AddSeconds(expirySeconds),
            Verb = HttpVerb.GET
        };
        return await s3.GetPreSignedURLAsync(request);
    }

    public async Task DeleteAsync(string key)
    {
        await s3.DeleteObjectAsync(_bucket, key);
    }

    public StorageKeyBuilder BuildKey { get; } = new();
}
