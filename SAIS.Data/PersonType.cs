using System;
using System.Collections.Generic;

namespace SAIS.Data
{
    public partial class PersonType
    {
        public PersonType()
        {
            People = new HashSet<Person>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public ICollection<Person> People { get; set; }
    }
}
