using CPG.Application.Shared.Resource;
using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Application.Exceptions;

public class ApplicationNotFoundException(long appId) : ApplicationException(string.Format(GlobalResource.ApplicationNotFound, appId))
{
    public override string Code => "application_not_found";
    public long AppId { get; } = appId;
}