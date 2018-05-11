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
    //AppointedPerson
    public class Register1Controller : BaseController
    {
        private readonly Register1Service _service;
        public Register1Controller(Register1Service service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            PersonModel model = await _service.Get(0);
            return View("PersonCreateEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PersonModel model)
        {
            return await SaveAsync(
                () => _service.CreateEdit(model),
                () => RedirectToAction(nameof(Index)),
                () => View("PersonCreateEdit", model)
            );
        }

        public string Test()
        {
            return _service.Test();
        }
    }
}
