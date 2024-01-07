using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Users.Queries
{
    public class GetUserQuery : IRequest<Result<UserViewModel>>
    {
     

    }
}
