using HotChocolate.Types;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;

namespace CPG.Infrastructure.Persistence.GraphQL.Types;

public class CompanyReadModelType : ObjectType<CompanyReadModel>
{
    protected override void Configure(IObjectTypeDescriptor<CompanyReadModel> descriptor)
    {
    }
}