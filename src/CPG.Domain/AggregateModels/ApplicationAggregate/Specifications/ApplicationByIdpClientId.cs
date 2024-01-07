using Ardalis.Specification;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications
{
    public class ApplicationByIdpClientId : Specification<Application>
    {
        public ApplicationByIdpClientId(string idpClientId)
        {
            Query
                .Include(app => app.ApplicationIdentifiers).Include(app => app.ApplicationCallbackUrls).
                Where(app => app.ApplicationIdentifiers.
                Select(t=> t.IdpClientId).Contains(idpClientId));
        }
    }
}
