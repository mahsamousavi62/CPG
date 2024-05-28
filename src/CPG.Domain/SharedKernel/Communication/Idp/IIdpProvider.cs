using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.Idp.Models.UserProfile;
using CPG.Domain.SharedKernel.Communication.Idp.Models.UserStatus;

namespace CPG.Domain.SharedKernel.Communication.Idp;
public interface IIdpProvider
{
    Task<ResultData<UserProfileResponse>> GetUserProfile(string idpId);
    Task<ResultData<UserStatusResponse>> GetUserStatus(string idpId);
}

