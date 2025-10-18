using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.File;
using CPG.Domain.SharedKernel.Minio;
using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
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

namespace CPG.Infrastructure.Minio;
public class MinioProvider : IMinioProvider
{
    private readonly IMinioClient _minioClient;
    private readonly IConfiguration _configuration;
    private readonly IMinioClientFactory _minioClientFactory;
    private readonly ILogService _logService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MinioProvider(IConfiguration configuration, IMinioClientFactory minioClientFactory, ILogService logService, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _minioClientFactory = minioClientFactory;
        _minioClient = _minioClientFactory.CreateClient();
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

        try
        {
            PutObjectArgs putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithContentType(file.ContentType)
                .WithObjectSize(file.Length)
                .WithStreamData(file.Content);

            var response = await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

            var resString = JsonConvert.SerializeObject(response);

            var callLog = CreateCallLogModel(nameof(PutObject), null, resString, true);
            _logService.LogWarning(callLog);

            return response.ObjectName;
        }
        catch (MinioException exc)
        {
            var callLog = CreateCallLogModel(nameof(PutObject), exc, exc.Message, false);
            _logService.LogError(callLog);
            return exc.Message;
        }
    }

    public async Task<FileViewModel> GetObjectByName(string name)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

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
            catch (Exception exc)
            {
                var callLog = CreateCallLogModel(nameof(GetObjectByName), exc, exc.Message, false);
                _logService.LogError(callLog);
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
            var callLog = CreateCallLogModel(nameof(GetObjectByName), exc, exc.Message, false);
            _logService.LogError(callLog);
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
            var callLog = CreateCallLogModel(nameof(PresignedGetObject), exc, exc.Message, false);
            _logService.LogError(callLog);

            throw new Exception($"{GlobalResource.MinioException} : {exc.Message}");
        }
    }

    private static string GetLocalFilePath(string destinationfilePath)
    {
        var directoryPath = Path.GetDirectoryName(destinationfilePath);

        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", directoryPath);
    }

    private CallLogModel CreateCallLogModel(string methodName, Exception exception, string responseBody, bool isSuccess)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long userId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

        var callTime = DateTime.Now;

        return new CallLogModel
        {
            // New fields (25 required fields)
            CorrelationId = httpContext?.TraceIdentifier,
            LogId = $"{Guid.NewGuid()} - Minio - {(isSuccess ? "Success" : exception?.GetType().Name)}",
            RequestId = httpContext?.TraceIdentifier,
            AuditLevel = isSuccess ? 1 : (exception != null ? 3 : 2),
            AuditType = Enums.AuditType.Provider,
            ServiceName = "Minio",
            ProviderName = "MinioProvider",
            RequestUri = methodName,
            RequestHeader = null,
            RequestBody = null,
            ResponseStatusCode = isSuccess ? 200 : 500,
            ResponseHeader = null,
            ResponseBody = responseBody,
            ApplicationId = applicationId == 0 ? null : applicationId,
            UserId = userId == 0 ? null : userId,
            Ip = httpContext?.Connection.RemoteIpAddress?.ToString(),
            CompanyId = companyId == 0 ? null : companyId,
            UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
            Response = responseBody,
            ErrorCode = exception?.GetType().Name,
            IsSucceeded = isSuccess,
            StartDateTime = callTime,
            EndDateTime = callTime,
            DurationMs = 0,
            StackTrace = exception?.StackTrace,

            // Original fields (preserved)
            ServiceCallDate = DateTime.Now,
            ServiceCallUrl = methodName,
            ServiceCallStatus = isSuccess,
            ServiceType = Enums.ServiceType.Minio,
            CreationDate = DateTime.Now,
            CreationUserId = userId == 0 ? 1 : userId,
            ErrorType = exception?.Message,
            CorrolationId = httpContext?.TraceIdentifier
        };
    }
}
