using System;
using System.Collections.Generic;

namespace SAIS.Data
{
    public partial class Document
    {
        public int Id { get; set; }
        public string RegisterTypeCode { get; set; }
        public int? RegisterId { get; set; }
        public string Notes { get; set; }
        public byte[] Data { get; set; }

        public RegisterType RegisterTypeCodeNavigation { get; set; }
    }
}
