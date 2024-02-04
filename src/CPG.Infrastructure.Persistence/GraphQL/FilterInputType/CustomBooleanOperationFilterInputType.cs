using HotChocolate.Data.Filters;
using HotChocolate.Types;

namespace CPG.Infrastructure.Persistence.GraphQL.FilterInputType;

public class CustomBooleanOperationFilterInputType : BooleanOperationFilterInputType
{
    protected override void Configure(IFilterInputTypeDescriptor descriptor)
    {
        descriptor.Operation(DefaultFilterOperations.Equals).Type<BooleanType>();        
        descriptor.Operation(DefaultFilterOperations.NotEquals).Type<BooleanType>();
    }
}