using System;
using System.Collections.Generic;
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
        public Enums.AuditType AuditType { get; set; }
        public long? CompanyId { get; set; }
        public long? ApplicationId { get; set; }
        public long? UserId { get; set; }
        public string ClientId { get; set; }
        public string StackTrace { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorCode { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public long DurationMs { get; set; }
        public KeyValuePair<string ,object>[] RoutValues { get; set; }
        public string MobilePhone { get; set; }
    }
}
