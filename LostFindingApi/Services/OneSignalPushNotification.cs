using LostFindingApi.Models.Real_Time;
using Mailjet.Client.Resources;
using OneSignal.RestAPIv3.Client;
using OneSignal.RestAPIv3.Client.Resources;
using OneSignal.RestAPIv3.Client.Resources.Notifications;
using System;
using System.Threading.Tasks;

namespace LostFindingApi.Services
{
    public class OneSignalPushNotification
    {
        public static async Task<string> OneSignalPushingNotification(Notification notification, Guid AppId, string restKey)
        {
            var Client = new OneSignalClient(restKey);

            var opt = new NotificationCreateOptions()
            {
                AppId = AppId,
                IncludePlayerIds = notification.DeviceIds,
            };
            opt.Headings.Add(LanguageCodes.English, notification.Title);
            opt.Contents.Add(LanguageCodes.English, notification.Content);
            NotificationCreateResult result = await Client.Notifications.CreateAsync(opt);

            return result.Id;
        }
    }
}
