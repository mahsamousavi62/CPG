using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using CPG.Domain.SharedKernel.File;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.File;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.Exceptions;

namespace CPG.Infrastructure.Minio;
public class MinioProvider : IMinioProvider
{
    private IMinioClient _client;
    private IConfiguration _configuration;

    public MinioProvider(IMinioClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    private IMinioClient GetClient()
    {
        if (_client != null)
            return _client;
        //await InitConfig();
        var client = new MinioClient()
        .WithEndpoint("192.168.1.100:9000")
        .WithCredentials("a0Eq9YFgC7MqrTZz8ijh", "EXMniwER0KhjC8ShNUBOGyWjitbna9jwpPQx0Hds")
            .WithSSL(false);

        _client = client.Build();
        return _client;
    }

    public async Task<List<string>> GetBucketNamesAsync(CancellationToken cancellationToken = default)
    {
        //var client = GetClient();
        var result = await _client.ListBucketsAsync(cancellationToken);
        return result.Buckets.Select(t => t.Name).ToList();
    }

    public async Task<string> PutObject(IFile file)
    {
        //var client = GetClient();
        var bucketName = _configuration["Minio:bucketName"];
        var objectName = $"Company/{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}_{file.FileName}";

        await file.CopyToAsync(file.Content);

        file.Content.Seek(0, System.IO.SeekOrigin.Begin);

        try
        {
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithContentType(file.ContentType)
                .WithObjectSize(file.Length)
                .WithStreamData(file.Content);

            var response = await _client.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

            return response.ObjectName;
        }
        catch (MinioException e)
        {
            return e.Message;
        }
    }

    public async Task<IFile> GetObjectByName(string name)
    {
        var bucketName = "cpg";

        try
        {
            //var client = GetClient();

            var getStateArgs = new StatObjectArgs().WithBucket(bucketName).WithObject(name);

            // Retrieve object information
            var objectInfo = await _client.StatObjectAsync(getStateArgs);

            var downloadStream = new MemoryStream();

            var gArgs = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithMatchETag(objectInfo.ETag).
                WithObject(objectInfo.ObjectName);

            gArgs.WithCallbackStream(x =>
            {
                x.CopyToAsync(downloadStream);
                downloadStream.Seek(0, SeekOrigin.Begin);
            });
            // Stream the object content directly to the response
            _ = await _client.GetObjectAsync(gArgs).ConfigureAwait(true);

            var result = new FileStreamResult(downloadStream, objectInfo.ContentType)
            { FileDownloadName = objectInfo.ObjectName };

            FormFileProxy file = new FormFileProxy
            {
                FileName = result.FileDownloadName,
                Content = result.FileStream,
                ContentType = result.ContentType
            };
            return file;
        }
        catch (MinioException e)
        {
            throw new Exception ($"Download Error: {e.Message}");
        }
    }

    public async Task<string> PresignedGetObject(string objectName)
    {
        var bucketName = "cpg";

        try
        {
            var client = GetClient();
            // Generate a presigned URL for the object
            var presignedUrl = await client.PresignedGetObjectAsync(
                new PresignedGetObjectArgs() .WithBucket(bucketName).
                WithObject(objectName).
                WithExpiry(15)
                );

            // Redirect the client to the presigned URL
            return presignedUrl;

        }
        catch (Exception e)
        {
            throw new Exception( $"An error occurred while generating the presigned URL: {e.Message}");
        }
    }

   
}
