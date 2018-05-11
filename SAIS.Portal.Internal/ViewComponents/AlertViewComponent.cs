using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SAIS.Portal.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAIS.Portal.Util
{
    public class AlertViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(ITempDataDictionary tempData)
        {
            await Task.CompletedTask;
            List<AlertUtil.AlertModel> model = AlertUtil.GetAlerts(tempData);
            return View(model);
        }
    }
}
