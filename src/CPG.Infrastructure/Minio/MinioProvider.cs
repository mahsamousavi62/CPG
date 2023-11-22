namespace CPG.Infrastructure.Minio
{
	using Charisma.NeoBank.Shared.Infrastructure.Provider;
	using Farsica.Framework.Data;
	using System.Threading.Tasks;
	using Minio;
	using Microsoft.Extensions.Configuration;
	using Minio.Exceptions;
	using static Farsica.Framework.Core.Constants;
	using Minio.AspNetCore;
	using System.Globalization;
	using System.Diagnostics.CodeAnalysis;
	using Microsoft.AspNetCore.Http;
	using Farsica.Framework.Core;

	public class MinioProvider : IStorageProvider
	{
		private readonly MinioClient minioClient;
		private readonly string? bucketName;
		private readonly Lazy<IHttpContextAccessor> httpContextAccessor;

		public MinioProvider(
			[NotNull] Lazy<IMinioClientFactory> minioClientFactory,
			[NotNull] Lazy<IConfiguration> configuration,
			[NotNull] Lazy<IHttpContextAccessor> httpContextAccessor)
		{
			minioClient = CreateClient(minioClientFactory);
			bucketName = configuration.Value["Minio:bucketName"];
			this.httpContextAccessor = httpContextAccessor;
		}


		public async Task<ResultData<string>> PutObjectAsync([NotNull] string filePath, string contentType)
		{
			try
			{
				var fileName = filePath[filePath.Replace("\\", "/", StringComparison.CurrentCulture)
					.IndexOf($"Users/{httpContextAccessor.Value.HttpContext.UserId()}", StringComparison.CurrentCulture)..];
				// Make a bucket on the server, if not already present.
				var beArgs = new BucketExistsArgs().WithBucket(bucketName);
				bool found = await minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
				if (!found)
				{
					var mbArgs = new MakeBucketArgs().WithBucket(bucketName);
					await minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
				}
				// Upload a file to bucket.
				var putObjectArgs = new PutObjectArgs()
					.WithBucket(bucketName)
					.WithObject(fileName)
					.WithFileName(filePath)
					.WithContentType(contentType);
				_ = await minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
				return new ResultData<string>
				{
					OperationResult = OperationResult.Succeeded,
					Data = fileName,
				};
			}
			catch (Exception e)
			{
				return new ResultData<string>
				{
					OperationResult = OperationResult.Failed,
					Error = e.Message,
				};
			}
		}

		public async Task<ResultData<string>> GetObjectAsync([NotNull] string filePath)
		{
			try
			{
				var folderPath = Path.GetDirectoryName(filePath);
				if (!Directory.Exists(folderPath))
				{
					_ = Directory.CreateDirectory(folderPath!);
				}
				var fileName = filePath[filePath.Replace("\\", "/", StringComparison.CurrentCulture)
					.IndexOf($"Users/{httpContextAccessor.Value.HttpContext.UserId()}", StringComparison.CurrentCulture)..];
				// Get a file from bucket.
				var getObjectArgs = new GetObjectArgs()
					.WithBucket(bucketName)
					.WithObject(fileName)
					.WithFile(filePath)
					.WithServerSideEncryption(null);
				_ = await minioClient.GetObjectAsync(getObjectArgs);
				return new ResultData<string>
				{
					OperationResult = OperationResult.Succeeded,
					Data = fileName,
				};
			}
			catch (MinioException e)
			{
				return new ResultData<string>
				{
					OperationResult = OperationResult.Failed,
					Error = e.Message,
				};
			}
		}

		public async Task<ResultData<string>> RemoveObjectAsync([NotNull] string filePath)
		{
			try
			{
				var fileName = filePath[filePath.Replace("\\", "/", StringComparison.CurrentCulture)
					.IndexOf($"Users/{httpContextAccessor.Value.HttpContext.UserId()}", StringComparison.CurrentCulture)..];
				// Remove a file from bucket.
				var removeObjectArgs = new RemoveObjectArgs()
					.WithBucket(bucketName)
					.WithObject(fileName);
				await minioClient.RemoveObjectAsync(removeObjectArgs).ConfigureAwait(false);
				return new ResultData<string>
				{
					OperationResult = OperationResult.Succeeded,
					Data = fileName,
				};
			}
			catch (MinioException e)
			{
				return new ResultData<string>
				{
					OperationResult = OperationResult.Failed,
					Error = e.Message,
				};
			}
		}

		private static MinioClient CreateClient(Lazy<IMinioClientFactory> minioClientFactory)
		{
			var current = Thread.CurrentThread.CurrentCulture;
			current.DateTimeFormat.Calendar = new GregorianCalendar();
			return minioClientFactory.Value.CreateClient();
		}
	}
}
