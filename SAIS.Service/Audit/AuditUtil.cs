using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SAIS.Model.Audit;
using SAIS.Data;
//using SAIS.Data;

namespace SAIS.Serice.Audit
{
    public static class AuditUtil
    {
        public static int Add(AuditContext db, AuditModel model)
        {
            SAIS.Data.Audit audit = CreateAudit(model);

            // Понякога броят на детайлите е много голям и, за да се добавят бързо, трябва да се изключи AutoDetectChanges.
            //bool originalAutoDetectSetting = db.Configuration.AutoDetectChangesEnabled;
            //db.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                db.Audits.Add(audit);
                foreach (AuditDetail detail in audit.AuditDetails)
                {
                    detail.Audit = audit;
                    db.AuditDetails.Add(detail);
                }
            }
            finally
            {
                //db.Configuration.AutoDetectChangesEnabled = originalAutoDetectSetting;
            }

            db.SaveChangesWithValidationExplained();
            int id = audit.Id;
            model.Id = id;
            return id;
        }

        public static void GetMissingEntityRecordIdFromDetails(this AuditModel audit)
        {
            if (audit.EntityRecordId == null && audit.EntityName != null)
            {
                AuditDetailModel logDetail = audit.AuditDetails.Where(ad => ad.EntityName == audit.EntityName).FirstOrDefault();
                if (logDetail != null)
                {
                    audit.EntityRecordId = logDetail.RecordId;
                }
            }
        }

        private static Data.Audit CreateAudit(AuditModel model)
        {
            return new Data.Audit()
            {
                DateTime = model.DateTime,
                IpAddress = model.IpAddress,
                Url = model.UrlAccessed,
                Data = model.Data,
                Duration = model.DurationTicks,
                UserName = model.UserName,
                UserId = model.UserId,
                Controller = model.Controller,
                Action = model.Action,
                SessionId = model.SessionId,
                RequestMethod = model.RequestMethod,
                AuditTypeCode = model.AuditTypeString,
                Notes = model.Notes,
                EntityName = model.EntityName,
                EntityRecordId = model.EntityRecordId,

                AuditDetails = model.AuditDetails != null ? model.AuditDetails.Select(d => CreateAuditDetail(d)).ToArray() : new AuditDetail[] { },
            };
        }
        private static AuditDetail CreateAuditDetail(AuditDetailModel model)
        {
            return new AuditDetail()
            {
                AuditDetailType = model.AuditDetailTypeString,
                EntityName = model.EntityName,
                RecordId = model.RecordId,
                PropertyName = model.PropertyName,
                OriginalValue = model.OriginalValue,
                NewValue = model.NewValue,
                OriginalValueDescription = model.OriginalValueDescription,
                NewValueDescription = model.NewValueDescription,
            };
        }

    }
}
