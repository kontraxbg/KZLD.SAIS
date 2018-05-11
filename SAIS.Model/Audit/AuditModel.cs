using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAIS.Model.Audit
{
    public class AuditModel
    { 
        public int? Id { get; set; }
        [Display(Name = "Допълнителни данни")]
        public string Data { get; set; }

        [Display(Name = "IP адрес")]
        public string IpAddress { get; set; }
        [Display(Name = "Сесия")]
        public string SessionId { get; set; }
        [Display(Name = "Дата и час")]
        public DateTime DateTime { get; set; }
        [Display(Name = "URL")]
        public string UrlAccessed { get; set; }
        [Display(Name = "Метод")]
        public string RequestMethod { get; set; }
        [Display(Name = "Потребител")]
        public string UserName { get; set; }
        [Display(Name = "Потребител")]
        public string UserId { get; set; }
        [Display(Name = "Контролер")]
        public string Controller { get; set; }
        [Display(Name = "Action")]
        public string Action { get; set; }
        [Display(Name = "Бележки")]
        public string Notes { get; set; }
        public long DurationTicks { get; set; }

        // Необходимо за grid-a - иначе няма как да се държи списъкът IQueryable
        [Display(Name = "Вид")]
        public string AuditTypeString { get; set; }

        //private string auditTypeString { get; set; }
        [Display(Name = "Вид")]
        public AuditTypeCode? AuditTypeCode
        {
            get
            {
                return AuditTypeString.ToEnumNullable<AuditTypeCode>();
            }
            set
            {
                AuditTypeString = value.ToString();
            }
        }

        //[Display(Name = "Заявка Id")]
        //public int? RequestId { get; set; }

        public int? PreviousId { get; set; }
        public byte[] PreviousHash { get; set; }

        [Display(Name = "Таблица")]
        public string EntityName { get; set; }
        [Display(Name = "Id")]
        public string EntityRecordId { get; set; }

        public List<AuditDetailModel> AuditDetails { get; set; }

        public AuditModel Clone()
        {
            return new AuditModel
            {
                Data = Data,
                IpAddress = IpAddress,
                SessionId = SessionId,
                DateTime = DateTime,
                UrlAccessed = UrlAccessed,
                RequestMethod = RequestMethod,
                UserName = UserName,
                UserId = UserId,
                Controller = Controller,
                Action = Action,
                Notes = Notes,
                DurationTicks = DurationTicks,
                AuditTypeString = AuditTypeString,
                //RequestId = RequestId,
                EntityName = EntityName,
                EntityRecordId = EntityRecordId,
            };
        }

        public byte[] Hash { get; set; }

        public override string ToString()
        {
            bool hasMasterEntity = !string.IsNullOrEmpty(EntityName) || !string.IsNullOrEmpty(EntityRecordId);
            return $"Id: {Id}, Data: {Data}, IpAddress: {IpAddress}, SessionId: {SessionId}, DateTime: {DateTime.ToString("yyyy-MM-dd hh:mm:ss")}, UrlAccessed: {UrlAccessed}, " +
                $"RequestMethod: {RequestMethod}, UserName: {UserName}, UserId: {UserId}, Controller: {Controller}, Action: {Action}, AuditTypeCode: {AuditTypeString.ToString()}, " +
                $"Details : {(AuditDetails == null ? "(null)" : string.Format("{{{0}}}", string.Join("," + Environment.NewLine, AuditDetails)))}" +
                (hasMasterEntity ? $", EntityName: {EntityName}, EntityRecordId: {EntityRecordId}" : null);
        }
    }
}
