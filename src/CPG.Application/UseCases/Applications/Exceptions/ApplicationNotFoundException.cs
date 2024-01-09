using CPG.Application.Shared.Resource;
using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.UseCases.Application.Exceptions;

public class ApplicationNotFoundException(long appId) : AppException(string.Format(GlobalResource.ApplicationNotFound, appId))
{
    public override string Code => "application_not_found";
    public long AppId { get; } = appId;
}