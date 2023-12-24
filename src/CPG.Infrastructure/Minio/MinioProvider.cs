using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel.File;
using CPG.Domain.SharedKernel.Minio;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Minio;
public class MinioProvider : IMinioProvider
{
    private readonly IMinioClient _minioClient;
    private readonly IConfiguration _configuration;
    private readonly IMinioClientFactory _minioClientFactory;


    public MinioProvider(
      IConfiguration configuration, IMinioClientFactory minioClientFactory)
    {
        _configuration = configuration;
        _minioClientFactory = minioClientFactory;
        _minioClient = _minioClientFactory.CreateClient();
    }

    public async Task<List<string>> GetBucketNamesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _minioClient.ListBucketsAsync(cancellationToken);
        return result.Buckets.Select(t => t.Name).ToList();
    }

    public async Task<string> PutObject(string uploadFromEntityType, IFile file)
    {
        var bucketName = _configuration["Infrastructure:Minio:bucketName"];
        var objectName = $"{uploadFromEntityType}/{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}_{file.FileName}";

        await file.ReadFile();

        file.Content.Seek(0, SeekOrigin.Begin);

        try
        {
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithContentType(file.ContentType)
                .WithObjectSize(file.Length)
                .WithStreamData(file.Content);

            var response = await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

            return response.ObjectName;
        }
        catch (MinioException e)
        {
            return e.Message;
        }
    }

    public async Task<FileViewModel> GetObjectByName(string name)
    {
        var bucketName = _configuration["Infrastructure:Minio:bucketName"];

        try
        {
            var getStateArgs = new StatObjectArgs().WithBucket(bucketName).WithObject(name);
            var objectInfo = await _minioClient.StatObjectAsync(getStateArgs);
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
            try
            {
                _ = await _minioClient.GetObjectAsync(gArgs).ConfigureAwait(true);
            }
            catch (Exception)
            {
                throw new Exception(GlobalResource.FileNotFound);
            }

            return new FileViewModel()
            {
                FileName = objectInfo.ObjectName,
                Content = downloadStream,
                byteArray = downloadStream.ToArray(),
                ContentType = objectInfo.ContentType
            };
        }
        catch (MinioException e)
        {
            throw new Exception($"{GlobalResource.MinioException} : {e.Message}");
        }
    }

    public async Task<string> PresignedGetObject(string objectName)
    {
        var bucketName = _configuration["Infrastructure:Minio:bucketName"];

        try
        {
            string localDestinationPath = GetLocalFilePath(objectName);
            var fileName = Path.GetFileName(objectName);

            if (!Directory.Exists(localDestinationPath))
            {
                _ = Directory.CreateDirectory(localDestinationPath);
            }

            var fullPath = Path.Combine(localDestinationPath, fileName);

            if (Path.Exists(fullPath))
            {
                return fullPath;
            }

            var getObjectArgs = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFile(fullPath);

            _ = await _minioClient.GetObjectAsync(getObjectArgs);

            return fullPath;
        }
        catch (Exception e)
        {
            throw new Exception($"{GlobalResource.MinioException} : {e.Message}");
        }
    }

    private static string GetLocalFilePath(string destinationfilePath)
    {
        var directoryPath = Path.GetDirectoryName(destinationfilePath);

        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\\Files\\", directoryPath.Replace('/', '\\'));
    }
}
