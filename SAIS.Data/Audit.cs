using System;
using System.Collections.Generic;

namespace SAIS.Data
{
    public partial class Audit
    {
        public Audit()
        {
            AuditDetails = new HashSet<AuditDetail>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime DateTime { get; set; }
        public string IpAddress { get; set; }
        public string Url { get; set; }
        public string Data { get; set; }
        public string Notes { get; set; }
        public long? Duration { get; set; }
        public string UserName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string SessionId { get; set; }
        public string RequestMethod { get; set; }
        public byte[] Hash { get; set; }
        public string AuditTypeCode { get; set; }
        public string EntityName { get; set; }
        public string EntityRecordId { get; set; }

        public AuditType AuditTypeCodeNavigation { get; set; }
        public ICollection<AuditDetail> AuditDetails { get; set; }
    }
}
