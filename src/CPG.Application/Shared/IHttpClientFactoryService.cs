using CPG.Domain.AggregateModels.UserAggregate.UserViewModel;
using System.Threading.Tasks;

namespace CPG.Application.Shared
{
    public interface IHttpClientFactoryService
    {
        Task<string> Execute(GetIdpUserProfileModel model);
    }
}
