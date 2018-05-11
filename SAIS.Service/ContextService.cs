using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SAIS.Data;
using SAIS.Model.Audit;
using SAIS.Serice.Audit;
using SAIS.Service.Audit;

namespace SAIS.Service
{
    public class ContextService
    {
        private SaisContext _db;
        private AuditContext _auditDb;
        private AuditModel _auditModel;

        public SaisContext Db
        {
            get
            {
                return _db;
            }
        }

        public ContextService(SaisContext db, AuditContext auditDb, AuditModel auditModel)
        {
            _db = db;
            _auditDb = auditDb;
            _auditModel = auditModel;
        }

        /// <summary>
        /// Опростен вариант за master entity-та с числово id. Спестява досадното писане на typeof(Entity) и id.ToString().
        /// </summary>
        public Task SaveAndLogAsync(object logMasterEntity, int logMasterEntityId)
        {
            if (logMasterEntity == null)
            {
                throw new ArgumentNullException(nameof(logMasterEntity));
            }
            return SaveAndLogAsync(_auditModel, logMasterEntity.GetType(), logMasterEntityId.ToString());
        }

        /// <summary>
        /// Използва се само при създаване на новo master entity.
        /// </summary>
        public Task SaveAndLogAsync(object logNewMasterEntity)
        {
            if (logNewMasterEntity == null)
            {
                throw new ArgumentNullException(nameof(logNewMasterEntity));
            }
            // Вторият параметър е null, защото entity-то още няма id, НО след _db.SaveChangesAsync() id-то се появява и
            // log.EntityRecordId се попълва автоматично с id-то на първия намерен обект от подадения тип.
            return SaveAndLogAsync(_auditModel, logNewMasterEntity.GetType(), null);
        }

        public Task SaveAndLogAsync(AuditModel audit, Type auditMasterEntityType, string auditMasterEntityId)
        {
            return ChangeLogger.SaveChangesAsync(_db, _auditDb, _db.SaveChangesWithValidationExplainedAsync, audit, auditMasterEntityType, auditMasterEntityId);
        }

        public int Log(AuditModel model)
        {
            return AuditUtil.Add(_auditDb, model);
        }
    }
}
