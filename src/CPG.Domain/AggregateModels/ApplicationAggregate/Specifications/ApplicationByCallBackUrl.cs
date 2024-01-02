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
    public class ApplicationByCallBackUrl : Specification<Application>
    {
        public ApplicationByCallBackUrl(string[] callbackUrls)
        {
            Query
                .Include(app => app.ApplicationCallbackUrls).
            Where(app => app.ApplicationCallbackUrls.Any(a => callbackUrls.Contains(a.CallbackUrl)));
        }
    }
}
