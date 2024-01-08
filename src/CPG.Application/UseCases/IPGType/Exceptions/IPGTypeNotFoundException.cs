using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.IPGTypes.Exceptions;

public class IPGTypeNotFoundException(long ipgTypeId) : AppException(string.Format(GlobalResource.IPGTypeNotFound, ipgTypeId))
{
    public override string Code => "ipgType_not_found";
    public long IPGTypeId { get; } = ipgTypeId;
}