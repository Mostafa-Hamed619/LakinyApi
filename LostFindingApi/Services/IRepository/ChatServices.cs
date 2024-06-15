using LostFindingApi.Models.DTO.RealTimeDTOs;
using LostFindingApi.Models.Real_Time;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace LostFindingApi.Services.IRepository
{
    public interface ChatServices
    {
        public Task AddMessage(Chat Message);

        public Task<IEnumerable<Chat>> GetChats(string senderId, string receiverId);

        public Task DeleteChats(string senderId, string receiverId);

        public Task<IEnumerable<getLastChatDTO>> GetLastChat(string senderId, string receiverId);
    }
}
