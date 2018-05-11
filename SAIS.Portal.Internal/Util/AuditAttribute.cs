using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using SAIS.Model.Audit;

namespace SAIS.Portal.Util
{
    public class AuditAttribute: ActionFilterAttribute
    {
        private readonly AuditModel _auditModel;
        public AuditAttribute(AuditModel auditModel)
        {
            _auditModel = auditModel;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _auditModel.Controller = filterContext.Controller.ToString();
        }
    }
}
