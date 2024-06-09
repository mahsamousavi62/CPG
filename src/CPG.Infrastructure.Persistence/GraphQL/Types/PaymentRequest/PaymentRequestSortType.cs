using CPG.Infrastructure.Persistence.GraphQL.Model;
using CPG.Infrastructure.Persistence.GraphQL.SortInputType;
using HotChocolate.Data.Sorting;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.PaymentRequest;

public class PaymentRequestSortType : SortInputType<PaymentRequestReportViewModel>
{
    protected override void Configure(ISortInputTypeDescriptor<PaymentRequestReportViewModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.Id).Type<AscDescSortEnumType>();
    }
}