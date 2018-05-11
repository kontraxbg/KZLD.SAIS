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
    public class BaseService
    {
        protected SaisContext _db;
        private ContextService _contextService;
        public BaseService(ContextService contextService)
        {
            _db = contextService.Db;
            _contextService = contextService;
        }

        public Task SaveAndLogAsync(object logMasterEntity, int logMasterEntityId)
        {
            return _contextService.SaveAndLogAsync(logMasterEntity, logMasterEntityId);
        }

        public Task SaveAndLogAsync(object logMasterEntity)
        {
            return _contextService.SaveAndLogAsync(logMasterEntity);
        }

        public static void MustExist(object o, string type, object id)
        {
            if (o == null)
            {
                throw new Exception($"Не съществува {type} с id {id}.");
            }
        }
    }
}
