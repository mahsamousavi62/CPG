using CPG.Infrastructure.Persistence.GraphQL.CustomFilterInputType;
using CPG.Infrastructure.Persistence.GraphQL.Model;
using HotChocolate.Data.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.CharismaCard;

    public class CharismaCardTransactionFilterType : FilterInputType<CharismaCardTransactionReportViewModel>
    {
        protected override void Configure(IFilterInputTypeDescriptor<CharismaCardTransactionReportViewModel> descriptor)
        {
            descriptor.BindFieldsExplicitly();
            descriptor.Field(f => f.CompanyId).Type<CustomLongOperationFilterInputType>();
            descriptor.Field(f => f.Id).Type<CustomLongOperationFilterInputType>();
            descriptor.Field(f => f.CompanyPersianName).Type<CustomStringOperationFilterInputType>();
            descriptor.Field(f => f.ReferenceNumber).Type<CustomStringOperationFilterInputType>();
            descriptor.Field(f => f.PaymentCode).Type<CustomStringOperationFilterInputType>();
            descriptor.Field(f => f.TransactionStatusCode).Type<CustomStringOperationFilterInputType>();
            descriptor.Field(f => f.TrackerId).Type<CustomStringOperationFilterInputType>();
            descriptor.Field(f => f.ProviderTrackerId).Type<CustomStringOperationFilterInputType>();
        }
    }
