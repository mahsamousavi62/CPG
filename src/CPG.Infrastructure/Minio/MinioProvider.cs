using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Common.Queries;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.File;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Authorization;
using CPG.Infrastructure.File;
using MassTransit.Mediator;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Minio;
public class MinioProvider : IMinioProvider
{
    private IMinioClient _client;
    private MediatR.IMediator _mediator;
    private readonly IConfiguration _configuration;

    public MinioProvider(MediatR.IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    private async Task<IMinioClient> GetClient()
    {
        if (_client != null)
            return _client;

        AuthenticationConfigViewModel config = await _mediator.Send(new GetAuthenticationAppSettingQuery());
        var client = new MinioClient()
        .WithEndpoint(config.Minio_EndPoint)
        .WithCredentials(config.Minio_AccessKey,config.Minio_SecretKey)
            .WithSSL(false);

        _client = client.Build();
        return _client;
    }

    public async Task<List<string>> GetBucketNamesAsync(CancellationToken cancellationToken = default)
    {
       _client =await GetClient();
        var result = await _client.ListBucketsAsync(cancellationToken);
        return result.Buckets.Select(t => t.Name).ToList();
    }

    public async Task<string> PutObject( string uploadFromEntityType, IFile file)
    {
        _client = await GetClient();
        var bucketName = _configuration["Infrastructure:Minio:bucketName"];
        var objectName = $"{uploadFromEntityType}/{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}_{file.FileName}";

        await file.ReadFile();

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

    public async Task<FileViewModel> GetObjectByName(string name)
    {
        var bucketName = _configuration["Infrastructure:Minio:bucketName"];

        try
        {
            _client =await GetClient();

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
            try
            {

            _ = await _client.GetObjectAsync(gArgs).ConfigureAwait(true);
            }
            catch (Exception)
            {

                throw new Exception("Not found FileName");
            }
            
           // var result = new FileStreamResult(downloadStream, objectInfo.ContentType)
           // { FileDownloadName = objectInfo.ObjectName };

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
            throw new Exception ($"Download Error: {e.Message}");
        }
    }

    public async Task<string> PresignedGetObject(string objectName)
    {
        var bucketName = _configuration["Infrastructure:Minio:bucketName"];

        try
        {
            var client =await GetClient();
            // Generate a presigned URL for the object
            var presignedUrl = await client.PresignedGetObjectAsync(
                new PresignedGetObjectArgs() .WithBucket(bucketName).
                WithObject(objectName)
                .WithExpiry(604800)
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
