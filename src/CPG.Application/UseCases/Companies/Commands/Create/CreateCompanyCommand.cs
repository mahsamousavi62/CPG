using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Commands.Create
{
    public class CreateCompanyCommand:IRequest
    {
        public CreateCompanyViewModel Model { get; set; }

        public CreateCompanyCommand(CreateCompanyViewModel model)
        {
            Model = model;
        }
    }
}
