using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SAIS.Data
{
    public partial class SaisContext
    {
        public SaisContext(DbContextOptions<SaisContext> options)
            : base(options)
        {
        }
    }
}
