using System;
using System.Collections.Generic;

namespace SAIS.Data
{
    public partial class AuditDetail
    {
        public int Id { get; set; }
        public int AuditId { get; set; }
        public string AuditDetailType { get; set; }
        public string EntityName { get; set; }
        public string RecordId { get; set; }
        public string PropertyName { get; set; }
        public string OriginalValue { get; set; }
        public string NewValue { get; set; }
        public string OriginalValueDescription { get; set; }
        public string NewValueDescription { get; set; }

        public Audit Audit { get; set; }
    }
}
