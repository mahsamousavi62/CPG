using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.Idp.Models.UserProfile;

namespace CPG.Domain.SharedKernel.Communication.Idp;
public interface IIdpProvider
{
    Task<ResultData<UserProfileResponse>> GetUserProfile(string idpId);
}

