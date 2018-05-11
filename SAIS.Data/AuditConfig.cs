using System;
using System.Collections.Generic;

namespace SAIS.Data
{
    public partial class AuditConfig
    {
        public string EntityName { get; set; }
        public string PropertyName { get; set; }
        public string Translation { get; set; }
        public string Mapping { get; set; }
    }
}
