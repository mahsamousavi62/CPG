using CPG.Application.Shared.Resource;
using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.UseCases.Users.Exceptions;

public class UserNotVerifyStatusException(string kycStatus) 
    : AppException(string.Empty)
{
    public override string Code => "user_not_verifyStatus";
    public string KycStatus { get; } = kycStatus;
}
