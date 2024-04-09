using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPG.Infrastructure.Persistence.GraphQL.CustomFilterInputType;
using CPG.Infrastructure.Persistence.GraphQL.Model;
using HotChocolate.Data.Filters;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.PaymentReceiptTransaction
{

    public class PaymentReceiptTransactionFilterType : FilterInputType<PaymentReceiptTransactionsReportViewModel>
    {
        protected override void Configure(IFilterInputTypeDescriptor<PaymentReceiptTransactionsReportViewModel> descriptor)
        {
            descriptor.BindFieldsExplicitly();
            descriptor.Field(f => f.CompanyId).Type<CustomLongOperationFilterInputType>();
            descriptor.Field(f => f.Id).Type<CustomLongOperationFilterInputType>();
            descriptor.Field(f => f.BankId).Type<CustomLongOperationFilterInputType>();
            descriptor.Field(f => f.SourceIban).Type<CustomStringOperationFilterInputType>();
            descriptor.Field(f => f.CompanyPersianName).Type<CustomStringOperationFilterInputType>();
            descriptor.Field(f => f.ReferenceNumber).Type<CustomStringOperationFilterInputType>();
            descriptor.Field(f => f.PaymentCode).Type<CustomStringOperationFilterInputType>();
            descriptor.Field(f => f.TransactionStatusCode).Type<CustomStringOperationFilterInputType>();
        }
    }
 
}
