using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.AggregateModels.UserAggregate.UserViewModel;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.ClientFactory;
    public interface IIdpClient
    {
    Task<ResultData<IdpUserProfile>> GetUserProfile(string idpId);
    }

