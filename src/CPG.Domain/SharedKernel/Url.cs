using Ardalis.GuardClauses;
using CPG.Domain.SharedKernel.Exceptions;
using System.Text.RegularExpressions;

namespace CPG.Domain.SharedKernel;

public class Url
{
    public string Value { get; }

    private Url()
    {
    }

    public Url(string url)
    {
        Guard.Against.NullOrWhiteSpace(url, nameof(url));

        if (url.Length < 10 || url.Length > 2048)
            throw new InvalidUrlCharacterLimitException(url);

        const string pattern = @"^(https?|http?):\/\/[^\s\/$.?#].[^\s]*$";
        if (!Regex.IsMatch(url, pattern))
            throw new InvalidUrlFormatException(url);
        Value = url;
    }

    public static implicit operator string(Url url) => url.Value;

    public static implicit operator Url(string value) => new(value);

    public override string ToString() => Value;
}
