using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.ClientFactory;
public interface IIdpClient
    {
    Task<ResultData<IdpUserProfile>> GetUserProfile(string idpId);
    }

