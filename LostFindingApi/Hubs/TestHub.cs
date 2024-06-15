using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LostFindingApi.Hubs
{
    public class TestHub : Hub
    {
        public async Task SendMessage(string User, string Message)
        {
            await Clients.All.SendAsync("ReceiveMessage", User, Message);
        }
    }
}
