using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Commands.Create
{
    public class CreateComapnyCommandHandler : IRequestHandler<CreateCompanyCommand, Unit>
    {

        private readonly IAggregateRepository<Company> _repository;

        public CreateComapnyCommandHandler(IAggregateRepository<Company> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            PersianName persianName = new(request.Model.PersianName);
            EnglishName englishName = new(request.Model.EnglishName);
            Logo logo = new("");

            var company = Company.Create(persianName, englishName, request.Model.NationalCodeMatchingRequied,
                                        logo, request.Model.MethodTypes);
            //await _repository.AddAsync(company);
            //await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
