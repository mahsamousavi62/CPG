using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.SortInputType;
using HotChocolate.Data.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.PaymentReceiptTransaction
{
    public class PaymentReceiptTransactionSortType : SortInputType<PaymentReceiptTransactionReadModel>
    {
        protected override void Configure(ISortInputTypeDescriptor<PaymentReceiptTransactionReadModel> descriptor)
        {
            descriptor.BindFieldsExplicitly();
            descriptor.Field(f => f.Id).Type<AscDescSortEnumType>();
            descriptor.Field(f => f.CreationDate).Type<AscDescSortEnumType>();
            descriptor.Field(f => f.ModificationDate).Type<AscDescSortEnumType>();
        }
    }
}
