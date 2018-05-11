using System.ComponentModel.DataAnnotations;

namespace SAIS.Model.Audit
{
    public class AuditDetailModel
    {
        public int Id { get; set; }

        // Необходимо за grid-a - иначе няма как да се държи списъкът IQueryable
        [Display(Name = "Вид")]
        public string AuditDetailTypeString { get; set; }

        //private string auditTypeString { get; set; }
        [Display(Name = "Вид")]
        public AuditDetailTypeCode? AuditDetailTypeCode
        {
            get
            {
                return AuditDetailTypeString.ToEnumNullable<AuditDetailTypeCode>();
            }
            set
            {
                AuditDetailTypeString = value.ToString();
            }
        }


        [Display(Name = "Таблица")]
        public string EntityName { get; set; }

        [Display(Name = "Id")]
        public string RecordId { get; set; }

        [Display(Name = "Поле")]
        public string PropertyName { get; set; }

        [Display(Name = "Стара стойност")]
        public string OriginalValue { get; set; }

        [Display(Name = "Нова стойност")]
        public string NewValue { get; set; }

        [Display(Name = "Стара стойност описание")]
        public string OriginalValueDescription { get; set; }

        [Display(Name = "Нова стойност описание")]
        public string NewValueDescription { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, AuditDetailType: {AuditDetailTypeCode}, EntityName: {EntityName}, RecordId: {RecordId}, PropertyName: {PropertyName}, " + 
                $"OriginalValue: {OriginalValue}, NewValue: {NewValue}, OriginalValueDescription: {OriginalValueDescription}, NewValueDescription: {NewValueDescription}";
        }
    }
}
