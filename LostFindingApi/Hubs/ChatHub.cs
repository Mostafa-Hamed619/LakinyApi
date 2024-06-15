using LostFindingApi.Models.Data;
using LostFindingApi.Models.Real_Time;
using LostFindingApi.Services.IRepository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LostFindingApi.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatServices chatServices;
        private readonly DataContext db;

        public ChatHub(ChatServices chatServices,DataContext db)
        {
            this.chatServices = chatServices;
            this.db = db;
        }

        public override async Task OnConnectedAsync()
        {
            // Handle client connection
            await base.OnConnectedAsync();
        }


        public async Task SendMessage(string message)
        {
            //  await Clients.User(senderId).SendAsync("ReceiveMessage", Message);
            // await Clients.User(recieverId).SendAsync("ReceiveMessage", Message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Handle client disconnection here
            // Log the disconnection event or clean up any resources associated with the client
            await base.OnDisconnectedAsync(exception);
        }

    }
}
