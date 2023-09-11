using HotChocolate.Types;
using Daryaftyar.Infrastructure.Persistence.DbContexts.ReadModels;

namespace Daryaftyar.Infrastructure.Persistence.GraphQL.Types
{
    public class BookReadModelType : ObjectType<BookReadModel>
    {
        protected override void Configure(IObjectTypeDescriptor<BookReadModel> descriptor)
        {
        }
    }
}