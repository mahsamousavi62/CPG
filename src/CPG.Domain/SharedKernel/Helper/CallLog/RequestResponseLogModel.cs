using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.IPGTypeAggregate;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Helper.CallLog
{
    public class RequestResponseLogModel
    {
        public DateTime RequestTime { get; set; }

        public string? RequestMethod { get; set; }

        public object? RequestBody { get; set; }

        public string? RequestQueryString { get; set; }

        public DateTime ResponseTime { get; set; }

        public string? ResponseStatus { get; set; }

        public object? ResponseBody { get; set; }

        public AuditType? AuditType { get; set; }
    }
}
