using System;
using System.ComponentModel.DataAnnotations;

namespace SAIS.Model.Audit
{
    public class AuditSearchModel
    {
        [Display(Name = "От дата/час")]
        [DataType(DataType.DateTime)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "до дата/час")]
        [DataType(DataType.DateTime)]
        public DateTime? ToDate { get; set; }
        [Display(Name = "Сесия")]
        public string SessionId { get; set; }
        [Display(Name = "Потребител")]
        public string UserName { get; set; }
        [Display(Name = "Вид")]

        // Nullable елементи, защото иначе когато се поства информацията с javascript, въпреки че не се поства стойност на патаметъра, AuditType, 
        // в модела се създава масив с един елемент със стойност 0 - (Default)
        public AuditTypeCode?[] AuditTypes { get; set; }

        public string EntityName { get; set; }
        public string EntityId{ get; set; }
        public string EntityDescription { get; set; }
         
        public string EntityTypeName { get; set; }
        public string ReturnUrl { get; set; }
        public string ReturnTitle { get; set; }

    }
}
