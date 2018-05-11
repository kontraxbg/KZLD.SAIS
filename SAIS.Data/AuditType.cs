using System;
using System.Collections.Generic;

namespace SAIS.Data
{
    public partial class AuditType
    {
        public AuditType()
        {
            Audits = new HashSet<Audit>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public ICollection<Audit> Audits { get; set; }
    }
}
