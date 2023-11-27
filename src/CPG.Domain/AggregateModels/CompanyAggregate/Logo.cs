using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.File;
using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate;

public class Logo
{
    private IMinioProvider _provider;

   

    public string Value { get; init; }

    public Logo(IFile file,string uploadFromEntityType, IMinioProvider provider)
    {
        _provider = provider;

        int maxFileSize = 2 * 1024 * 1024;

        Guard.Against.Null(file, nameof(file));

        if (file.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
            throw new InvalidLogoException(nameof(file));

        Guard.Against.NegativeOrZero(file.Length, nameof(file));

        if (!Regex.IsMatch(Path.GetExtension(file.FileName), "^.*\\.(jpg|JPG|gif|jpeg|png|tiff|svg)$"))
            throw new InvalidLogoExtentionException($"Parameter {nameof(file)} has a  invalid extention.");

        if (file.Length > maxFileSize)
            throw new MaximalFileSizeException("MaximalFileSize");
       
        Value = UploadFile(uploadFromEntityType,file);

    }

    public  string UploadFile(string uploadFromEntityType, IFile file)
    {
        var result =  _provider.PutObject(uploadFromEntityType, file).GetAwaiter().GetResult();
       
        return result;
    }

}