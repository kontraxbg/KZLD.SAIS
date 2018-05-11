using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SAIS.Model;
using SAIS.Model.Audit;

namespace SAIS.Portal.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, AuditModel auditModel)
        {
            try
            {
                auditModel.UserName = context.User.Identity.Name;
                //auditModel.Controller = contextAccessor.HttpContext.
                await _next(context);
            }
            catch (Exception ex)
            {
                //await TryLogAsync(context, ex);

                // Оригиналният exception се подава към следващия middleware,
                throw ex;
            }
        }
    }
}
