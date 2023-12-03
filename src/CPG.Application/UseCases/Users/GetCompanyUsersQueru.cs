using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.AggregateModels.UserAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Users
{
    public class GetCompanyUsersQuery:IRequest<IReadOnlyCollection<UserViewModel>>
    {
    }
}
