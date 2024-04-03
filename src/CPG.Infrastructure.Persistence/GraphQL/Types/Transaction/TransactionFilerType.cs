using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.CustomFilterInputType;
using CPG.Infrastructure.Persistence.GraphQL.FilterInputType;
using CPG.Infrastructure.Persistence.GraphQL.Model;
using HotChocolate.Data.Filters;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.Transaction;

public class TransactionFilerType : FilterInputType<TransactionReportViewModel>
{
    protected override void Configure(IFilterInputTypeDescriptor<TransactionReportViewModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.CompanyId).Type<CustomLongOperationFilterInputType>();
        descriptor.Field(f => f.Id).Type<CustomLongOperationFilterInputType>();
        descriptor.Field(f => f.CompanyPersianName).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.TransactionMethodType).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.NationalCode).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.ApplicationId).Type<CustomLongOperationFilterInputType>();
        descriptor.Field(f => f.FirstName).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.LastName).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.CompanyDepositaccountNumber).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.CompanyDepositIban).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.PaymentCode).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.Status).Type<CustomStringOperationFilterInputType>();


    }
}
