using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SAIS.Model;

namespace SAIS.Portal.Util
{
    public static class AlertUtil
    {
        public class AlertModel
        {
            private readonly string _alertStyle;
            private readonly string _message;
            private readonly bool _isDismissable;

            public AlertModel(string alertStyle, string message, bool isDismissable)
            {
                _alertStyle = alertStyle;
                _message = message;
                _isDismissable = isDismissable;
            }

            public string AlertStyle
            {
                get { return _alertStyle; }
            }

            public string Message
            {
                get { return _message; }
            }

            public bool IsDismissable
            {
                get { return _isDismissable; }
            }
        }

        private const string _tempDataKey = "TempDataAlerts";

        private const string _classSuccess = "success";
        private const string _classInformation = "info";
        private const string _classWarning = "warning";
        private const string _classDanger = "danger";

        public static void Success(ITempDataDictionary tempData, string message, bool isDismissable)
        {
            AddAlert(tempData, _classSuccess, message, isDismissable);
        }

        public static void Information(ITempDataDictionary tempData, string message, bool isDismissable)
        {
            AddAlert(tempData, _classInformation, message, isDismissable);
        }

        public static void Warning(ITempDataDictionary tempData, string message, bool isDismissable)
        {
            AddAlert(tempData, _classWarning, message, isDismissable);
        }

        public static void Danger(ITempDataDictionary tempData, string message, bool isDismissable)
        {
            AddAlert(tempData, _classDanger, message, isDismissable);
        }

        /// <summary>
        /// Добавяне на съобщение
        /// </summary>
        /// <param name="alertStyle"></param>
        /// <param name="message">Съобщение</param>
        /// <param name="dismissable">Премахваемо</param>
        private static void AddAlert(ITempDataDictionary tempData, string alertStyle, string message, bool isDismissable)
        {
            List<AlertModel> alerts = GetAlerts(tempData);
            alerts.Add(new AlertModel(alertStyle, message, isDismissable));
            tempData[_tempDataKey] = JsonConvert.SerializeObject(alerts);
            //когато напишат да може да се сериализират други неща, освен прости типове
            //tempData[_tempDataKey] = alerts;
        }

        public static List<AlertModel> GetAlerts(ITempDataDictionary tempData)
        {
            return tempData.ContainsKey(_tempDataKey)
                ? JsonConvert.DeserializeObject<List<AlertModel>>((string)tempData[_tempDataKey])
                : new List<AlertModel>();
            //когато напишат да може да се сериализират други неща, освен прости типове
            //return tempData.ContainsKey(_tempDataKey)
            //    ? (List<AlertModel>)tempData[_tempDataKey]
            //    : new List<AlertModel>();
        }
    }
}
