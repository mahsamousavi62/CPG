using CPG.Domain.Exceptions;

namespace CPG.Domain.SharedKernel.Exceptions;

internal class InvalidUrlCharacterLimitException(string message) : DomainException(string.Format(Resource.InvalidUrlCharacterLimit, message))
{
    public override string Code => "invalid_url_character_limit";
}
