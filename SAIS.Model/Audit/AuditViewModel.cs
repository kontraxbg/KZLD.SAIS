using System;
using System.ComponentModel.DataAnnotations;

namespace SAIS.Model.Audit
{
    public class AuditViewModel : AuditModel
    {
        [Display(Name = "Дата и час")]
        public string DateTimeText
        {
            get { return DateTime.ToString("dd.MM.yy HH:mm:ss").Replace(" ", "&nbsp;"); }
        }

        [Display(Name = "Време")]
        public string DurationText
        {
            get
            {
                TimeSpan duration = TimeSpan.FromTicks(DurationTicks);
                return duration.TotalSeconds > 1.0
                    ? duration.ToString("h':'mm':'ss")
                    : duration != TimeSpan.Zero
                        ? (int)duration.TotalMilliseconds + " ms"
                        : null;
            }
        }

        [Display(Name = "Вид")]
        public string AuditTypeOrRequestMethod
        {
            get
            {
                string m = RequestMethod;
                return AuditTypeCode == Audit.AuditTypeCode.Read && m != null && m.ToLower() != "get" ? m : AuditTypeName;
            }
        }

        [Display(Name = "Вид")]
        public string AuditTypeName { get; set; }
    }
}
