using LostFindingApi.Models.Real_Time;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LostFindingApi.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task NotificationToAll(Notification notification)
        {
            await Clients.All.SendAsync("NotificationSent", notification);
        }
    }
}
