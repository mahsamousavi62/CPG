using CPG.Domain.SharedKernel.File;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.Minio;

public interface IMinioProvider
{
    Task<string> PutObject(IFile file);

    Task<IFile> GetObjectByName(string objectName);

    Task<string> PresignedGetObject(string objectName);   
}
