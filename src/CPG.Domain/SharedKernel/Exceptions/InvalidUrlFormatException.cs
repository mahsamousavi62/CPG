using CPG.Domain.Exceptions;

namespace CPG.Domain.SharedKernel.Exceptions;

internal class InvalidUrlFormatException(string message) : DomainException(string.Format(Resource.InvalidUrlFormat, message))
{
    public override string Code => "invalid_url_format";
}