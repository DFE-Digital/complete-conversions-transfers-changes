using Dfe.Complete.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Dfe.Complete.Extensions
{
    public static class TempDataExtensions
    {
        public static void SetNotification(this ITempDataDictionary tempData, NotificationType notificationType, string notificationTitle, string notificationMessage)
        {
            tempData["NotificationType"] = notificationType.ToString().ToLower();
            tempData["NotificationTitle"] = notificationTitle;
            tempData["NotificationMessage"] = notificationMessage;
        }
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T? Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            if (tempData.TryGetValue(key, out var value) && value is string json)
            {
                return JsonConvert.DeserializeObject<T>(json);
            }

            return null;
        }
        public static T? Peek<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            return tempData.Peek(key) is not string json ? null : JsonConvert.DeserializeObject<T>(json);
        }
    }
}
