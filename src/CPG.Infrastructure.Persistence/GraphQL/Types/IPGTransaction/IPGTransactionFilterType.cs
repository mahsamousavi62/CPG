
using CPG.Infrastructure.Persistence.GraphQL.CustomFilterInputType;
using CPG.Infrastructure.Persistence.GraphQL.Model;
using HotChocolate.Data.Filters;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.IPGTransaction;

public class IPGTransactionFilterType : FilterInputType<IpgTransactionReportViewModel>
{
    protected override void Configure(IFilterInputTypeDescriptor<IpgTransactionReportViewModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.CompanyId).Type<CustomLongOperationFilterInputType>();
        descriptor.Field(f => f.Id).Type<CustomLongOperationFilterInputType>();
        descriptor.Field(f => f.IPGTypeId).Type<CustomLongOperationFilterInputType>();
        descriptor.Field(f => f.CompanyPersianName).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.ReferenceNumber).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.PaymentCode).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.TransactionStatusCode).Type<CustomStringOperationFilterInputType>();
    }
}