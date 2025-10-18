using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.File;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Minio;
public class MinioProvider : IMinioProvider
{
    private readonly IMinioClient _minioClient;
    private readonly IConfiguration _configuration;
    private readonly IMinioClientFactory _minioClientFactory;
    private readonly ILogger<MinioProvider> _logger;

    public MinioProvider(IConfiguration configuration, IMinioClientFactory minioClientFactory, ILogger<MinioProvider> logger)
    {
        _configuration = configuration;
        _minioClientFactory = minioClientFactory;
        _minioClient = _minioClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<List<string>> GetBucketNamesAsync(CancellationToken cancellationToken = default)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        var result = await _minioClient.ListBucketsAsync(cancellationToken);
        return result.Buckets.Select(t => t.Name).ToList();
    }

    public async Task<string> PutObject(string uploadFromEntityType, IFile file)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        var bucketName = _configuration["Infrastructure:Minio:bucketName"];
        var sanitizedFileName = Path.GetFileName(file.FileName);
        var objectName = $"{uploadFromEntityType}/{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}_{sanitizedFileName}";

        await file.ReadFile();

        file.Content.Seek(0, SeekOrigin.Begin);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            PutObjectArgs putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithContentType(file.ContentType)
                .WithObjectSize(file.Length)
                .WithStreamData(file.Content);

            var response = await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
            stopwatch.Stop();

            // لاگ ساختاریافته - به جای LogWarning
            _logger.LogInformation(
                "[MinIO] PutObject succeeded in {DurationMs}ms | Bucket: {Bucket} | ObjectKey: {ObjectKey} | Size: {Size} bytes",
                stopwatch.ElapsedMilliseconds, bucketName, objectName, file.Length);

            return response.ObjectName;
        }
        catch (MinioException exc)
        {
            stopwatch.Stop();

            _logger.LogError(exc,
                "[MinIO] PutObject FAILED after {DurationMs}ms | Bucket: {Bucket} | ObjectKey: {ObjectKey} | Size: {Size} bytes",
                stopwatch.ElapsedMilliseconds, bucketName, objectName, file.Length);

            return exc.Message;
        }
    }

    public async Task<FileViewModel> GetObjectByName(string name)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        var bucketName = _configuration["Infrastructure:Minio:bucketName"];

        var stopwatch = Stopwatch.StartNew();
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
                stopwatch.Stop();

                _logger.LogInformation(
                    "[MinIO] GetObject succeeded in {DurationMs}ms | Bucket: {Bucket} | ObjectKey: {ObjectKey}",
                    stopwatch.ElapsedMilliseconds, bucketName, name);
            }
            catch (Exception exc)
            {
                stopwatch.Stop();

                _logger.LogError(exc,
                    "[MinIO] GetObject FAILED after {DurationMs}ms | Bucket: {Bucket} | ObjectKey: {ObjectKey}",
                    stopwatch.ElapsedMilliseconds, bucketName, name);

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
        catch (MinioException exc)
        {
            stopwatch.Stop();

            _logger.LogError(exc,
                "[MinIO] GetObject MinioException after {DurationMs}ms | Bucket: {Bucket} | ObjectKey: {ObjectKey}",
                stopwatch.ElapsedMilliseconds, bucketName, name);

            throw new Exception($"{GlobalResource.MinioException} : {exc.Message}");
        }
    }

    public async Task<string> PresignedGetObject(string objectName)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        var bucketName = _configuration["Infrastructure:Minio:bucketName"];
        var serviceUrl = _configuration["ApiServerUrl"];

        try
        {
            string localDestinationPath = GetLocalFilePath(objectName);
            var fileName = Path.GetFileName(objectName);

            if (!Directory.Exists(localDestinationPath))
            {
                _ = Directory.CreateDirectory(localDestinationPath);
            }

            var fullPath = Path.Combine(localDestinationPath, fileName);

            if (Path.Exists(fullPath) is false)
            {
                var getObjectArgs = new GetObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithFile(fullPath);

                _ = await _minioClient.GetObjectAsync(getObjectArgs);
            }

            return serviceUrl + objectName; // Path.Combine("wwwroot", objectName);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, $"Request: Unhandled Exception for Request {nameof(PresignedGetObject)}{exc.Message} ");

            throw new Exception($"{GlobalResource.MinioException} : {exc.Message}");
        }
    }

    private static string GetLocalFilePath(string destinationfilePath)
    {
        var directoryPath = Path.GetDirectoryName(destinationfilePath);

        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", directoryPath);
    }
}
