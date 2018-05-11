using System;
using System.Collections.Generic;

namespace SAIS.Data
{
    public partial class Addres
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string AddressTypeCode { get; set; }
        public string Name { get; set; }

        public AddressType AddressTypeCodeNavigation { get; set; }
        public Company Company { get; set; }
    }
}
