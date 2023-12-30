using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using CPG.Application.UseCases.Ipg.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Ipg.Queries
{
    public class ReturnToOriginByCodeQuery(ReturnToOriginByCodeViewModel model) : IRequest<string>
    {
        public ReturnToOriginByCodeViewModel model = model;
    }
}
