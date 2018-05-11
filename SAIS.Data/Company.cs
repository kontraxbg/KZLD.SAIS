using System;
using System.Collections.Generic;

namespace SAIS.Data
{
    public partial class Company
    {
        public Company()
        {
            Addres = new HashSet<Addres>();
            People = new HashSet<Person>();
        }

        public int Id { get; set; }
        public bool Verified { get; set; }
        public string Name { get; set; }

        public ICollection<Addres> Addres { get; set; }
        public ICollection<Person> People { get; set; }
    }
}
