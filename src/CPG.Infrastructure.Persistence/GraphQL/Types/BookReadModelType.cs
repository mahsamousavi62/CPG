using HotChocolate.Types;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;

namespace CPG.Infrastructure.Persistence.GraphQL.Types;

public class BookReadModelType : ObjectType<BookReadModel>
{
    protected override void Configure(IObjectTypeDescriptor<BookReadModel> descriptor)
    {
    }
}