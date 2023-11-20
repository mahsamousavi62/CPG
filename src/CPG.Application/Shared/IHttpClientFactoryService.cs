using CPG.Domain.AggregateModels.UserAggregate.UserViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.Shared
{
    public interface IHttpClientFactoryService
    {
        Task<string> Execute(GetIdpUserProfileModel model);
    }
}
