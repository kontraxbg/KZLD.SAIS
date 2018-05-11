using System;
using System.Collections.Generic;
using System.Text;

namespace SAIS.Model
{
    public class IdNameModel
    {
        private readonly int _id;
        private readonly string _name;

        public IdNameModel(int id, string name)
        {
            _id = id;
            _name = name;
        }

        public int Id { get => _id; }

        public string Name { get => _name; }
    }
}
