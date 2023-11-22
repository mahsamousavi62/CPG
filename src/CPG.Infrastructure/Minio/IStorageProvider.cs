using CPG.Domain.SharedKernel;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Minio
{
    public interface IStorageProvider
    {
        Task<ResultData<string>> PutObjectAsync(string filePath, string contentType);

        Task<ResultData<string>> GetObjectAsync(string filePath);

        Task<ResultData<string>> RemoveObjectAsync(string filePath);
    }
}
