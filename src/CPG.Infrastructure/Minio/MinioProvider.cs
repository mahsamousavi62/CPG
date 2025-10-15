using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.File;
using CPG.Domain.SharedKernel.Minio;
using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CPG.Infrastructure.Minio;
public class MinioProvider : IMinioProvider
{
    private readonly IMinioClient _minioClient;
    private readonly IConfiguration _configuration;
    private readonly IMinioClientFactory _minioClientFactory;
    private readonly ILogger<MinioProvider> _logger;
    private readonly ILogService _logService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MinioProvider(
        IConfiguration configuration,
        IMinioClientFactory minioClientFactory,
        ILogger<MinioProvider> logger,
        ILogService logService,
        IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _minioClientFactory = minioClientFactory;
        _minioClient = _minioClientFactory.CreateClient();
        _logger = logger;
        _logService = logService;
        _httpContextAccessor = httpContextAccessor;
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
        var startTime = DateTime.UtcNow;

        // Extract user context
        var httpContext = _httpContextAccessor.HttpContext;
        var correlationId = httpContext?.Items["CorrelationId"]?.ToString();
        var requestId = httpContext?.Items["RequestId"]?.ToString();

        long? userId = null;
        long? companyId = null;
        long? applicationId = null;

        if (httpContext?.User?.Claims != null)
        {
            var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var uid))
                userId = uid;

            var companyIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            if (!string.IsNullOrEmpty(companyIdClaim) && long.TryParse(companyIdClaim, out var cid))
                companyId = cid;

            var appIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value;
            if (!string.IsNullOrEmpty(appIdClaim) && long.TryParse(appIdClaim, out var aid))
                applicationId = aid;
        }

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

            // Log successful MinIO operation
            _logService.LogMinioOperation(new MinioOperationLog
            {
                OperationType = "PutObject",
                BucketName = bucketName,
                ObjectName = objectName,
                FileSizeBytes = file.Length,
                ContentType = file.ContentType,
                EntityType = uploadFromEntityType,
                IsSuccess = true,
                StartDateTime = startTime,
                EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                DurationMs = stopwatch.ElapsedMilliseconds,
                UserId = userId,
                CompanyId = companyId,
                ApplicationId = applicationId,
                CorrelationId = correlationId,
                RequestId = requestId
            });

            return response.ObjectName;
        }
        catch (MinioException exc)
        {
            stopwatch.Stop();

            // Log failed MinIO operation
            _logService.LogMinioOperation(new MinioOperationLog
            {
                OperationType = "PutObject",
                BucketName = bucketName,
                ObjectName = objectName,
                FileSizeBytes = file.Length,
                ContentType = file.ContentType,
                EntityType = uploadFromEntityType,
                IsSuccess = false,
                ErrorCode = exc.GetType().Name,
                ErrorMessage = exc.Message,
                StartDateTime = startTime,
                EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                DurationMs = stopwatch.ElapsedMilliseconds,
                UserId = userId,
                CompanyId = companyId,
                ApplicationId = applicationId,
                CorrelationId = correlationId,
                RequestId = requestId
            });

            _logger.LogError(exc, "Unhandled exception in MinIO operation {Operation}", nameof(PutObject));
            return exc.Message;
        }
    }

    public async Task<FileViewModel> GetObjectByName(string name)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        var bucketName = _configuration["Infrastructure:Minio:bucketName"];
        var stopwatch = Stopwatch.StartNew();
        var startTime = DateTime.UtcNow;

        // Extract user context
        var httpContext = _httpContextAccessor.HttpContext;
        var correlationId = httpContext?.Items["CorrelationId"]?.ToString();
        var requestId = httpContext?.Items["RequestId"]?.ToString();

        long? userId = null;
        long? companyId = null;
        long? applicationId = null;

        if (httpContext?.User?.Claims != null)
        {
            var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var uid))
                userId = uid;

            var companyIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            if (!string.IsNullOrEmpty(companyIdClaim) && long.TryParse(companyIdClaim, out var cid))
                companyId = cid;

            var appIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value;
            if (!string.IsNullOrEmpty(appIdClaim) && long.TryParse(appIdClaim, out var aid))
                applicationId = aid;
        }

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

                // Log successful MinIO operation
                _logService.LogMinioOperation(new MinioOperationLog
                {
                    OperationType = "GetObject",
                    BucketName = bucketName,
                    ObjectName = name,
                    FileSizeBytes = objectInfo.Size,
                    ContentType = objectInfo.ContentType,
                    IsSuccess = true,
                    StartDateTime = startTime,
                    EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    UserId = userId,
                    CompanyId = companyId,
                    ApplicationId = applicationId,
                    CorrelationId = correlationId,
                    RequestId = requestId
                });
            }
            catch (Exception exc)
            {
                stopwatch.Stop();

                // Log failed MinIO operation
                _logService.LogMinioOperation(new MinioOperationLog
                {
                    OperationType = "GetObject",
                    BucketName = bucketName,
                    ObjectName = name,
                    IsSuccess = false,
                    ErrorCode = exc.GetType().Name,
                    ErrorMessage = exc.Message,
                    StartDateTime = startTime,
                    EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    UserId = userId,
                    CompanyId = companyId,
                    ApplicationId = applicationId,
                    CorrelationId = correlationId,
                    RequestId = requestId
                });

                _logger.LogError(exc, "Unhandled exception in MinIO operation {Operation} for object {ObjectName}", nameof(GetObjectByName), name);
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

            // Log failed MinIO operation
            _logService.LogMinioOperation(new MinioOperationLog
            {
                OperationType = "GetObject",
                BucketName = bucketName,
                ObjectName = name,
                IsSuccess = false,
                ErrorCode = exc.GetType().Name,
                ErrorMessage = exc.Message,
                StartDateTime = startTime,
                EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                DurationMs = stopwatch.ElapsedMilliseconds,
                UserId = userId,
                CompanyId = companyId,
                ApplicationId = applicationId,
                CorrelationId = correlationId,
                RequestId = requestId
            });

            _logger.LogError(exc, "MinIO exception in operation {Operation} for object {ObjectName}", nameof(GetObjectByName), name);
            throw new Exception($"{GlobalResource.MinioException} : {exc.Message}");
        }
    }

    public async Task<string> PresignedGetObject(string objectName)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        var bucketName = _configuration["Infrastructure:Minio:bucketName"];
        var serviceUrl = _configuration["ApiServerUrl"];
        var stopwatch = Stopwatch.StartNew();
        var startTime = DateTime.UtcNow;

        // Extract user context
        var httpContext = _httpContextAccessor.HttpContext;
        var correlationId = httpContext?.Items["CorrelationId"]?.ToString();
        var requestId = httpContext?.Items["RequestId"]?.ToString();

        long? userId = null;
        long? companyId = null;
        long? applicationId = null;

        if (httpContext?.User?.Claims != null)
        {
            var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var uid))
                userId = uid;

            var companyIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            if (!string.IsNullOrEmpty(companyIdClaim) && long.TryParse(companyIdClaim, out var cid))
                companyId = cid;

            var appIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value;
            if (!string.IsNullOrEmpty(appIdClaim) && long.TryParse(appIdClaim, out var aid))
                applicationId = aid;
        }

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

            stopwatch.Stop();

            // Log successful MinIO operation
            _logService.LogMinioOperation(new MinioOperationLog
            {
                OperationType = "PresignedGetObject",
                BucketName = bucketName,
                ObjectName = objectName,
                IsSuccess = true,
                StartDateTime = startTime,
                EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                DurationMs = stopwatch.ElapsedMilliseconds,
                UserId = userId,
                CompanyId = companyId,
                ApplicationId = applicationId,
                CorrelationId = correlationId,
                RequestId = requestId
            });

            return serviceUrl + objectName; // Path.Combine("wwwroot", objectName);
        }
        catch (Exception exc)
        {
            stopwatch.Stop();

            // Log failed MinIO operation
            _logService.LogMinioOperation(new MinioOperationLog
            {
                OperationType = "PresignedGetObject",
                BucketName = bucketName,
                ObjectName = objectName,
                IsSuccess = false,
                ErrorCode = exc.GetType().Name,
                ErrorMessage = exc.Message,
                StartDateTime = startTime,
                EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                DurationMs = stopwatch.ElapsedMilliseconds,
                UserId = userId,
                CompanyId = companyId,
                ApplicationId = applicationId,
                CorrelationId = correlationId,
                RequestId = requestId
            });

            _logger.LogError(exc, "Unhandled exception in MinIO operation {Operation} for object {ObjectName}", nameof(PresignedGetObject), objectName);

            throw new Exception($"{GlobalResource.MinioException} : {exc.Message}");
        }
    }

    private static string GetLocalFilePath(string destinationfilePath)
    {
        var directoryPath = Path.GetDirectoryName(destinationfilePath);

        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", directoryPath);
    }
}
