using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAIS.Model;
using SAIS.Portal.Models;
using SAIS.Portal.Util;
using SAIS.Service;

namespace SAIS.Portal.Controllers
{
    [RequireClientCertificate]
    public class CertificateController : BaseController
    {
        public IActionResult Index()
        {

            return View();
        }
    }
}
