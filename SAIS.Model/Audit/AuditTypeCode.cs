using System.ComponentModel.DataAnnotations;

namespace SAIS.Model.Audit
{
    public enum AuditTypeCode
    {
        /// <summary>
        /// Четене
        /// </summary>
        [Display(Name = "Четене")]
        Read,

        /// <summary>
        /// Запис
        /// </summary>
        [Display(Name = "Запис")]
        Write,

        /// <summary>
        /// Успешен вход
        /// </summary>
        [Display(Name = "Успешен вход")]
        LoginSuccess,

        /// <summary>
        /// Грешка при вход
        /// </summary>
        [Display(Name = "Грешка при вход")]
        LoginError,

        /// <summary>
        /// Заявка към RegiX
        /// </summary>
        [Display(Name = "Заявка към RegiX")]
        RegiXRequest,

        /// <summary>
        /// Отговор от Regix
        /// </summary>
        [Display(Name = "Отговор от RegiX")]
        RegiXResponse,

        /// <summary>
        /// Отговор от Regix
        /// </summary>
        [Display(Name = "Грешка от RegiX")]
        RegiXError,
    }

    public enum AuditDetailTypeCode
    {
        C,
        U,
        D,
    }
}
