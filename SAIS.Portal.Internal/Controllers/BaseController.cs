using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAIS.Portal.Models;

using System.Text;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SAIS.Portal.Util;
using System.Transactions;

namespace SAIS.Portal.Controllers
{
    [ServiceFilter(typeof(AuditAttribute))]
    public abstract class BaseController : Controller
    {
        protected string _saveSuccessMessage;

        #region Save

        protected Task<IActionResult> SaveAsync(Func<Task> saveAsync, Func<IActionResult> success, Func<IActionResult> fail)
        {
            return SaveAsync(saveAsync, () => Task.FromResult(success()), () => Task.FromResult(fail()));
        }

        protected Task<IActionResult> SaveAsync(Func<Task> saveAsync, Func<IActionResult> success, Func<Task<IActionResult>> failAsync)
        {
            return SaveAsync(saveAsync, () => Task.FromResult(success()), failAsync);
        }

        /// <summary>
        /// Общ вид на action за записване в базата данни.
        /// </summary>
        protected async Task<IActionResult> SaveAsync(Func<Task> saveAsync, Func<Task<IActionResult>> successAsync, Func<Task<IActionResult>> failAsync)
        {
            if (saveAsync == null)
            {
                throw new ArgumentNullException("saveAsync");
            }
            if (successAsync == null)
            {
                throw new ArgumentNullException("successAsync");
            }
            if (failAsync == null)
            {
                throw new ArgumentNullException("failAsync");
            }

            if (!ModelState.IsValid)
            {
                Warning(string.Join("<br />", ModelState.Values.SelectMany(s => s.Errors)
                    .Where(e => !string.IsNullOrEmpty(e.ErrorMessage) || e.Exception != null)
                    .Select(e => !string.IsNullOrEmpty(e.ErrorMessage) ? e.ErrorMessage : e.Exception.Message)));
                return await failAsync();
            }

            // TODO: Добавяне на TransactionScope - за момента го няма в .net core.
            try
            {
                await saveAsync();

                string successMessage = _saveSuccessMessage ?? "Записът е успешен.";
                if (successMessage.Length > 0)  // Чрез изрично подаване на string.Empty съобщението може да се потисне.
                {
                    Success(successMessage, true);
                }

                try
                {
                    return await successAsync();
                }
                catch (Exception ex)
                {
                    await LogAndShowErrorAsync("Възникна грешка!", ex);
                    return await failAsync();
                }
            }
            catch (Exception ex)
            {
                await LogAndShowErrorAsync("Грешка при записване на данните!", ex);
                return await failAsync();
            }
        }

        #endregion

        #region Alerts

        /// <summary>
        /// Предефиниран метод за добавяне на съобщение за успех
        /// </summary>
        /// <param name="message">Съобщение</param>
        /// <param name="isDismissable">Премахваемо</param>
        protected void Success(string message, bool isDismissable = false)
        {
            AlertUtil.Success(TempData, message, isDismissable);
        }

        /// <summary>
        /// Предефиниран метод за добавяне на съобщение за информация
        /// </summary>
        /// <param name="message">Съобщение</param>
        /// <param name="isDismissable">Премахваемо</param>
        protected void Information(string message, bool isDismissable = false)
        {
            AlertUtil.Information(TempData, message, isDismissable);
        }

        /// <summary>
        /// Предефиниран метод за добавяне на съобщение за предупреждение
        /// </summary>
        /// <param name="message">Съобщение</param>
        /// <param name="isDismissable">Премахваемо</param>
        protected void Warning(string message, bool isDismissable = false)
        {
            AlertUtil.Warning(TempData, message, isDismissable);
        }

        /// <summary>
        /// Предефиниран метод за добавяне на съобщение за опасност
        /// </summary>
        /// <param name="message">Съобщение</param>
        /// <param name="isDismissable">Премахваемо</param>
        protected void Danger(string message, bool isDismissable = false)
        {
            AlertUtil.Danger(TempData, message, isDismissable);
        }

        /// <summary>
        /// Използва се в catch блок, за да запише грешката в базата данни и да я покаже на екрана в Danger панел.
        /// </summary>
        protected async Task LogAndShowErrorAsync(string prefix, Exception ex)
        {
            Danger(await LogAndGetErrorAsync(prefix, ex));
        }

        /// <summary>
        /// За случаите при извикване с ajax, когато няма смисъл от стандартните alert функции.
        /// </summary>
        protected async Task<string> LogAndGetErrorAsync(string prefix, Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                // TODO: Добавяне на TransactionScope - за момента го няма в .net core.
                //// Записването на грешки не трябва да се rollback-ва. В случай че сме в scope, той не трябва да се ползва по време на логването.
                //await Services.ExceptionDbLoggerMiddleware.TryLogAsync(HttpContext, ex);
                scope.Complete();
            }

            StringBuilder text = new StringBuilder(ex.Message);
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                text.Insert(0, ex.Message + "; ");
            }
            if (!string.IsNullOrEmpty(prefix))
            {
                text.Insert(0, prefix + ": ");
            }

            return text.ToString();
        }

        #endregion

        #region ViewData/ViewBag забранени

        public const string UseModelsInsteadOfViewData = "Ползвай модели вместо ViewData!";
        public const string UseModelsInsteadOfViewBag = "Ползвай модели вместо ViewBag!";

        /// <summary>
        /// Ползването на ViewData е лоша практика, затова property-то е анотирано, така че да извежда грешка
        /// на компилатора. Също така гърми runtime.
        /// Property-то ViewData на view-тата извежда само warning, защото се ползва вътрешно от Razor.
        /// </summary>
        [Obsolete(UseModelsInsteadOfViewData, true)]
        public new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary ViewData
        {
            get => throw new Exception(UseModelsInsteadOfViewData);
            set => throw new Exception(UseModelsInsteadOfViewData);
        }

        /// <summary>
        /// Ползването на ViewBag е лоша практика, затова property-то е анотирано, така че да извежда грешка
        /// на компилатора. Също така гърми runtime.
        /// </summary>
        [Obsolete(UseModelsInsteadOfViewBag, true)]
        public new dynamic ViewBag
        {
            get => throw new Exception(UseModelsInsteadOfViewBag);
        }

        #endregion
    }
}
