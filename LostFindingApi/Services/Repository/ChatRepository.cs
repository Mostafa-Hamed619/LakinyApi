using LostFindingApi.Models.Data;
using LostFindingApi.Models.DTO.RealTimeDTOs;
using LostFindingApi.Models.Real_Time;
using LostFindingApi.Services.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace LostFindingApi.Services.Repository
{
    public class ChatRepository : ChatServices
    {
        private readonly DataContext db;

        public ChatRepository(DataContext db)
        {
            this.db = db;
        }

        public async Task AddMessage(Chat Message)
        {
            await db.chats.AddAsync(Message);
            await db.SaveChangesAsync();
        }

        public async Task DeleteChats(string senderId, string receiverId)
        {
            var chat = await db.chats
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) || (m.SenderId == receiverId && m.ReceiverId == senderId)).ToListAsync();
            
            foreach(var ch in chat)
            {
                db.Remove(ch);
            }
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Chat>> GetChats(string senderId, string receiverId)
        {
            var result =await  db.chats
            .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) || (m.SenderId == receiverId && m.ReceiverId == senderId))
            .Select(C => new Chat{ SenderId = C.SenderId, ReceiverId = C.ReceiverId, Content = C.Content,File = C.File,Latitude = C.Latitude,Longitude = C.Longitude })
            .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<getLastChatDTO>> GetLastChat(string senderId, string receiverId)
        {
            var result = await db.chats.Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) || (m.SenderId == receiverId && m.ReceiverId == senderId))
                .Select(c => new getLastChatDTO
                {
                    id = c.Id,
                    content = c.Content,
                    Time = c.Timestamp
                }).OrderByDescending(c => c.id).Take(1).ToListAsync();

            return result;
        }
    }
}
