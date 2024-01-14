using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Companies.Exceptions;

public class RequiredShaparakSettingsException() : AppException(string.Format(GlobalResource.RequiredShaparakSettings))
{
    public override string Code => "shaparak-settings-is-required";    
}