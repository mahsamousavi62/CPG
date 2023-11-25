using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.File;

public interface IFile
{
    string FileName { get; }
    string ContentType { get; }
    long Length { get; }
    Task CopyToAsync(Stream target);
    Task<byte[]> GetData();
    public Stream Content { get; set; }
    public string Extension => Path.GetExtension(this.FileName);

}
