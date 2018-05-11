using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SAIS.Data
{
    public partial class AuditContext
    {
        public AuditContext(DbContextOptions<AuditContext> options)
            : base(options)
        {
        }
    }
}
