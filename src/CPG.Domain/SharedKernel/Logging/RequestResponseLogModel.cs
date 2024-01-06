using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Logging
{
    public class RequestResponseLogModel
    {
        public string UserAgent { get; set; }
        public string IP { get; set; }
        public string Host { get; set; }
        public string ServiceName { get; set; }
        public DateTime RequestTime { get; set; }
        public string? RequestMethod { get; set; }
        public object? RequestBody { get; set; }
        public string? RequestQueryString { get; set; }
        public DateTime ResponseTime { get; set; }
        public string? ResponseStatus { get; set; }
        public object? ResponseBody { get; set; }
        public string AuditType { get; set; }
        public long? CompanyId { get; set; }
        public long? ApplicationId { get; set; }
        public long? UserId { get; set; }
        public long? ElapsedMilliseconds { get; set; }
        public string ClientId { get; set; }
    }
}
