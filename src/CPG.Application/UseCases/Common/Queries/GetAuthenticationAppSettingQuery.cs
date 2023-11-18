using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Authorization;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Common.Queries
{
    public class GetAuthenticationAppSettingQuery:IRequest<AuthenticationConfigViewModel>
    {
        public Enums.ApplicationSettingEntityType EntityType { get; } = Enums.ApplicationSettingEntityType.IDPCredential;
    }
}
