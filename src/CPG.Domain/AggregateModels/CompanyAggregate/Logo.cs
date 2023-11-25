using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.SharedKernel.File;
using System.IO;
using System.Text.RegularExpressions;

namespace CPG.Domain.AggregateModels.CompanyAggregate;

public class Logo
{
    public string Value { get; init; }

    public Logo(IFile file)
    {
        int maxFileSize = 2 * 1024 * 1024;

        Guard.Against.Null(file, nameof(file));

        if (file.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
            throw new InvalidLogoException(nameof(file));

        Guard.Against.NegativeOrZero(file.Length, nameof(file));

        if (!Regex.IsMatch(Path.GetExtension(file.FileName), "^.*\\.(jpg|JPG|gif|jpeg|png|tiff|svg)$"))
            throw new InvalidLogoExtentionException($"Parameter {nameof(file)} has a  invalid extention.");

        if (file.Length > maxFileSize)
            throw new MaximalFileSizeException("MaximalFileSize");

        Value = file.FileName;

    }
}