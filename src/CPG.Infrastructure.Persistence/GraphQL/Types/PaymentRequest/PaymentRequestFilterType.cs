using CPG.Infrastructure.Persistence.GraphQL.CustomFilterInputType;
using CPG.Infrastructure.Persistence.GraphQL.Model;
using HotChocolate.Data.Filters;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.PaymentRequest;

public class PaymentRequestFilterType : FilterInputType<PaymentRequestReportViewModel>
{
    protected override void Configure(IFilterInputTypeDescriptor<PaymentRequestReportViewModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.CompanyId).Type<CustomLongOperationFilterInputType>();
        descriptor.Field(f => f.Id).Type<CustomLongOperationFilterInputType>();
        descriptor.Field(f => f.NationalCode).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.ApplicationId).Type<CustomLongOperationFilterInputType>();
        descriptor.Field(f => f.PaymentCode).Type<CustomStringOperationFilterInputType>();
    }
}
