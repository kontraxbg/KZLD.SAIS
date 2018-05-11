using System;
using System.Collections.Generic;

namespace SAIS.Data
{
    public partial class Person
    {
        public int Id { get; set; }
        public bool Verified { get; set; }
        public int CompanyId { get; set; }
        public string PersonTypeCode { get; set; }
        public string PidType { get; set; }
        public string Identifier { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }

        public Company Company { get; set; }
        public PersonType PersonTypeCodeNavigation { get; set; }
    }
}
