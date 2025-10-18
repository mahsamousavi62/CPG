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

            var httpContext = _httpContextAccessor.HttpContext;
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long userId);
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

            var callLog = CallLogModel.CreateSuccess(
                serviceName: "Minio",
                providerName: "MinioProvider",
                requestUri: nameof(PutObject),
                requestBody: null,
                responseBody: resString,
                serviceType: Enums.ServiceType.Minio,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
            );
            _logService.LogWarning(callLog);

            return response.ObjectName;
        }
        catch (MinioException exc)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long userId);
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

            var callLog = CallLogModel.CreateError(
                serviceName: "Minio",
                providerName: "MinioProvider",
                requestUri: nameof(PutObject),
                requestBody: null,
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.Minio,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
            );
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
                var httpContext = _httpContextAccessor.HttpContext;
                _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long userId);
                _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
                _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

                var callLog = CallLogModel.CreateError(
                    serviceName: "Minio",
                    providerName: "MinioProvider",
                    requestUri: nameof(GetObjectByName),
                    requestBody: null,
                    responseBody: exc.Message,
                    exception: exc,
                    serviceType: Enums.ServiceType.Minio,
                    auditType: Enums.AuditType.Provider,
                    correlationId: httpContext?.TraceIdentifier,
                    userId: userId,
                    applicationId: applicationId,
                    companyId: companyId,
                    ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                    userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
                );
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
            var httpContext = _httpContextAccessor.HttpContext;
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long userId);
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

            var callLog = CallLogModel.CreateError(
                serviceName: "Minio",
                providerName: "MinioProvider",
                requestUri: nameof(GetObjectByName),
                requestBody: null,
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.Minio,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
            );
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
            var httpContext = _httpContextAccessor.HttpContext;
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long userId);
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

            var callLog = CallLogModel.CreateError(
                serviceName: "Minio",
                providerName: "MinioProvider",
                requestUri: nameof(PresignedGetObject),
                requestBody: null,
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.Minio,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
            );
            _logService.LogError(callLog);

            throw new Exception($"{GlobalResource.MinioException} : {exc.Message}");
        }
    }

    private static string GetLocalFilePath(string destinationfilePath)
    {
        var directoryPath = Path.GetDirectoryName(destinationfilePath);

        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", directoryPath);
    }
}
