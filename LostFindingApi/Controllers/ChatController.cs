using LostFindingApi.Hubs;
using LostFindingApi.Models;
using LostFindingApi.Models.DTO.RealTimeDTOs;
using LostFindingApi.Models.Real_Time;
using LostFindingApi.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostFindingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> logger;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly ChatServices chatServices;
        private readonly UserManager<User> _userManager;
        private readonly IFileRepository fileRepository;

        public ChatController(ILogger<ChatController> _logger,IHubContext<ChatHub> hubContext, ChatServices chatServices,UserManager<User> userManager,IFileRepository fileRepository)
        {
            logger = _logger;
            this.hubContext = hubContext;
            this.chatServices = chatServices;
            this._userManager = userManager;
            this.fileRepository = fileRepository;
        }

        [HttpGet("Get-All-Chat")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetAllChat(string senderEmail,string receiverEmail)
        {
            var user = await _userManager.FindByEmailAsync(senderEmail);
            
            if(user == null)
            {
                Log.Warning("the user is not existing.");
                return Ok("User is not existing");
            }
            var _senderId = user.Id;

            var _receiverId = (await _userManager.FindByEmailAsync(receiverEmail)).Id;
            

            var result =await chatServices.GetChats(_senderId, _receiverId);

            Log.Debug("getting all chat between {@user1} and {@user2}", senderEmail, receiverEmail);
            return Ok(result);
        }

        [HttpPost("add-Message-Chat")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var chatMessage = new Chat();
            if (model.File != null)
            {
                var fileResult = fileRepository.SaveChatFile(model.File);

                if (fileResult.Item1 == 1)
                {
                    chatMessage.File = fileResult.Item2;
                }
            }

            string _senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var _receiver =await _userManager.FindByEmailAsync(model.ReceiverEmail);

            chatMessage.SenderId = _senderId;
            chatMessage.ReceiverId = _receiver.Id;
            chatMessage.Content = model.Message;
            chatMessage.Latitude = model.Latitude;
            chatMessage.Longitude = model.Longitude;
                
            await chatServices.AddMessage(chatMessage);
            await hubContext.Clients.All.SendAsync("ReceiveMessage", model.Message);

            return Ok("Chat is ok");
        }

        [HttpDelete("delete-chat")]
        public async Task<IActionResult> DeleteChat(string ReceiverId)
        {
            string SenderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await chatServices.DeleteChats(SenderId, ReceiverId);
            await chatServices.DeleteChats(ReceiverId, SenderId);
            ApiResponse apiResponse = new ApiResponse();
            apiResponse.Status = true;
            apiResponse.Message = "The chat is deleted";
            return Ok(apiResponse);
        }

        [HttpGet("Get-last-chat")]
        public async Task<IActionResult> GetLastChat(string recieverId)
        {
            string senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await chatServices.GetLastChat(senderId,recieverId);
            
            if(result.Count() == 0)
            {
                SerializedChat chatJson = new SerializedChat
                {
                    content = null,
                };
                return Ok(chatJson);
            }
            else
            {
                SerializedChat chatJson = new SerializedChat
                {
                    content = result.FirstOrDefault().content,
                    Time = result.FirstOrDefault().Time
                };
                if (chatJson.content == null)
                {
                    chatJson.content = string.Empty;
                }

                logger.LogDebug("getting last chat");
                return Ok(chatJson);
            }
          
        }
    }
}
