using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SAIS.Model.Audit;

namespace SAIS.Portal.Util
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireClientCertificateAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
#if DEBUG
            // dig out cert from kestrel pipeline
            X509Certificate2 certificate = context.HttpContext.Connection.ClientCertificate;
            if (certificate == null)
            {
                // missing cert
                context.Result = new UnauthorizedResult();
            }
            else
            {
                Trace.WriteLine(certificate.SubjectName.Name, "RequireClientCertificate");
                // TODO: authorize
            }
#else
            // Azure App Service will pass base64 encoded certificate in a header
            string header = context.HttpContext.Request.Headers["X-ARR-ClientCert"];
            if (String.IsNullOrEmpty(header))
            {
                // missing cert
                context.Result = new UnauthorizedResult();
            }
            else
            {
                byte[] data = Convert.FromBase64String(header);
                using (var certificate = new X509Certificate2(data)) {
                    Trace.WriteLine(certificate.SubjectName.Name, "RequireClientCertificate");
                }
                // TODO: authorize
            }
#endif
        }
    }
}
