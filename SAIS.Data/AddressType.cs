using System;
using System.Collections.Generic;

namespace SAIS.Data
{
    public partial class AddressType
    {
        public AddressType()
        {
            Addres = new HashSet<Addres>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public ICollection<Addres> Addres { get; set; }
    }
}
