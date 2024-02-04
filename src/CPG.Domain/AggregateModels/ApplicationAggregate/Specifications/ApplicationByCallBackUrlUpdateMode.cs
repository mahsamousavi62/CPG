using Ardalis.Specification;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications
{
    public class ApplicationByCallBackUrlUpdateMode : Specification<Application>
    {
        public ApplicationByCallBackUrlUpdateMode(string[] callbackUrls,int id)
        {
            Query
                .Include(app => app.ApplicationCallbackUrls).
            Where(app => app.ApplicationCallbackUrls.Any(a => callbackUrls.Contains(a.CallbackUrl)) && app.Id!=id);
        }
    }
}
