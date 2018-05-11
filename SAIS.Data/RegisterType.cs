using System;
using System.Collections.Generic;

namespace SAIS.Data
{
    public partial class RegisterType
    {
        public RegisterType()
        {
            Documents = new HashSet<Document>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public ICollection<Document> Documents { get; set; }
    }
}
