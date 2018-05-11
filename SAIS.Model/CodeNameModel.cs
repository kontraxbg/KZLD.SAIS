using System;
using System.Collections.Generic;
using System.Text;

namespace SAIS.Model
{
    public class CodeNameModel
    {
        private readonly string _code;
        private readonly string _name;

        public CodeNameModel(string code, string name)
        {
            _code = code;
            _name = name;
        }

        public string Code { get => _code; }

        public string Name { get => _name; }
    }
}
