using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPG.Application.UseCases.Users.ViewModel;
using MediatR;

namespace CPG.Application.UseCases.Users.Queries
{
    public class GetUserQuery(string idpId) : IRequest<UserViewModel>
    {
        public string IdpId { get; set; } = idpId;
     

    }
}
