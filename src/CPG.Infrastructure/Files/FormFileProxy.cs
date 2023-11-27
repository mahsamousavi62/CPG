using Ardalis.GuardClauses;
using CPG.Domain.SharedKernel.File;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;


namespace CPG.Infrastructure.File;

public class FormFileProxy : IFile
{
    public FormFileProxy()
    {
        
    }
    private readonly IFormFile _formFile;
    public string ContentType
    {
        get
        {
            return _formFile.ContentType;
        }
        set { }
    }

    public long Length => _formFile.Length;

    public string FileName { get => _formFile.FileName; set { } }

    public Stream Content { get ; set ; }=new MemoryStream();

    public FormFileProxy(IFormFile formFile)
    {
        Guard.Against.Null(formFile, nameof(formFile));
        _formFile = formFile;
    }

    public async Task ReadFile()
    {
        using (var fileStream = _formFile.OpenReadStream())
        {
         await   fileStream.CopyToAsync(Content);
        }
    }
    public Task CopyToAsync(Stream target)
    {
        
        return _formFile.CopyToAsync(target);
    }
    
    public async Task<byte[]> GetData()
    {
        using var stream = new MemoryStream();
        await CopyToAsync(stream);
        return stream.ToArray();
    }
}
