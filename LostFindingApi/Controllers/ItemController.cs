using LostFindingApi.Hubs;
using LostFindingApi.Models;
using LostFindingApi.Models.Data;
using LostFindingApi.Models.DTO.ItemDTOs;
using LostFindingApi.Models.DTO.MLDTOs;
using LostFindingApi.Models.Real_Time;
using LostFindingApi.Services;
using LostFindingApi.Services.IRepository;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LostFindingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ItemController : ControllerBase
    {
        private readonly ItemRepository ItemRepository;
        private readonly IFileRepository fileRepository;
        private readonly IHubContext<NotificationHub> notificationHub;
        private readonly UserManager<Models.User> userManager;
        private readonly ICategoryRepository category;
        private readonly DataContext db;
        private readonly ILogger<ItemController> logger;
        private readonly IConfiguration configuration;
        private readonly HttpClient _httpClient;
        

        
    public ItemController(ItemRepository ItemRepository,IFileRepository fileRepository,IHubContext<NotificationHub> notificationHub,
            UserManager<Models.User> userManager,
            ICategoryRepository category,
            DataContext db,ILogger<ItemController> logger, IConfiguration configuration)
        {
            this.ItemRepository = ItemRepository;
            this.fileRepository = fileRepository;
            this.notificationHub = notificationHub;
            this.userManager = userManager;
            this.category = category;
            this.db = db;
            this.logger = logger;
            this.configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://32c9-102-189-19-54.ngrok-free.app/");


        }

        [HttpGet("Send-Notification")]
        public async Task<ActionResult> CreateNotification()
        {
            var result = ItemRepository.LastPosted();
            return Ok(result);
        }


        [HttpPost("Add-Item")]
        public async Task<ActionResult<Item>> Add([FromForm]AddItemDTO model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid process");
            }
            var user = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (user == null)
            {
                ApiResponse apiResponse = new ApiResponse();
                apiResponse.Message = "Access Denied ";
                apiResponse.Status = false;
                return BadRequest(apiResponse);
            }
            Item NewItem = new Item();
          
            if (model.ImagePhoto != null)
            {
                var fileResult = fileRepository.SaveImage(model.ImagePhoto);

                if ( fileResult.Item1 == 1)
                {

                    NewItem.ImagePhoto = fileResult.Item2;
                }
            }
            NewItem.Name = model.Name.ToLower();


            NewItem.Award = (model.Award == null) ? "0" : model.Award;
            NewItem.FoundDate = DateTime.Now.ToString("dddd dd MMMM");
            NewItem.Description = model.Description.ToLower();
            NewItem.Status = model.Status.ToLower();
            NewItem.CategoryId = category.GetCategoryId(model.CategoryName);
            NewItem.FoundPlace = model.FoundPlace.ToLower();
            NewItem.userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            NewItem.Latitude =  model.Latitude;
            NewItem.Longitude =  model.Longitude;

            var ItemResult = await ItemRepository.AddItemAsync(NewItem);

        

            
         
            ApiResponse apiresponse = new ApiResponse();
            apiresponse.Message = "Addition Successfully done";
            apiresponse.Status = true;


            Log.Debug("{@user} has added new item in the {@category} category with name {@name}", user.Email, model.CategoryName, model.Name);
            return Ok(new JsonResult(new
            {
                Result = apiresponse,
                item = NewItem
            }));

            
        }

        [HttpGet("get-Item-User-Details")]
        public ActionResult<object> GetAll()
        {
            var user = User.FindFirst(ClaimTypes.Email)?.Value;
            Log.Debug($"{user} has enter to get the users items detailed");
            var result = ItemRepository.GetAllItemCategory();
            if(result == null) { return BadRequest("No item exists yet"); }
            return Ok(result);
          
        }


        [HttpGet("get-item/{id}")]
        public async Task<ActionResult<getitemDTO>> GetItem(int id)
        {
            var result = await ItemRepository.getItem(id);
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (result == null) return NotFound($"item with id {id} is not found");
            Log.Debug("{@user} has enter to get item of result  {@result} sir",userEmail, result);


            return Ok(result);
        }

        [HttpDelete("delete-item/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var image = ItemRepository.getItemImage(id);
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (image == null) return NotFound($"No item with id {id}");
            
            else
            {
                Log.Debug("{@user} has deleted item{@item} from the application", userEmail, ItemRepository.getItem(id));
                ItemRepository.DeleteItemAsync(id);
                return Ok(new JsonResult(new
                {
                    title = "Deletion successfully done.",
                    message = "Your item is deleted successfully."
                }));
            }
        }

        [HttpPost("search-item/{search}")]
        public async Task<ActionResult<object>> Search(string search)
        {
            var result = await ItemRepository.GetBySearch(search);
            
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (result == null) return NotFound($"No item with {search} Name");

            Log.Debug("{@user} has searched for {@item} in the application", userEmail, search);
            
            return Ok(result);
        }

        [HttpGet("Filter-status/{status}")]
        public async Task<ActionResult<IEnumerable<Item>>> FilterStatus(string status)
        {
            var result = await ItemRepository.GetStatusFilter(status);
            if (result == null) return Ok($"No item {status} yet.");
            else
            {
                return Ok(result);
            }
        }

        [HttpGet("My-posts")]
        public async Task<ActionResult<IEnumerable<Item>>> GetUserPosts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByIdAsync(userId);
            Log.Debug("{@user} is on his posts ", user.Email);
            var result = await ItemRepository.getUserItems(userId);
            return Ok(result);
        }

        [HttpPost("Compare-Text-ML")]
        public async Task<IActionResult> CompareTextML(CompareTextDTO model)
        {
            model.founded_objects = ItemRepository.GetDescription();
            try
            {
                var apiUrl = "https://32c9-102-189-19-54.ngrok-free.app/find_objects";
                var response = await _httpClient.PostAsJsonAsync(apiUrl, model);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(responseData);

                var descriptions = jsonObject["top_matches"]
                                    .OrderByDescending(match => (double)match["similarity_score"])
                                    .Select(match => match["description"].ToString())
                                    .ToList();

                foreach (var item in db.Item)
                {
                    var match = jsonObject["top_matches"].FirstOrDefault(m => (string)m["description"] == item.Description);
                    if (match != null)
                    {
                        item.similarity_score = (double)match["similarity_score"];
                    }
                }
                var items = db.Item
                   .Join(
                       db.Users,
                       i => i.userId,
                       u => u.Id,
                       (i, u) => new { user = u, item = i }
                       )
                   .Select(ui => new
                   {
                       itemId = ui.item.Id,
                       itemStatus = ui.item.Status,
                       itemImage = ui.item.ImagePhoto,
                       itemName = ui.item.Name,
                       itemAward = ui.item.Award,
                       Description = ui.item.Description,
                       foundPlace = ui.item.FoundPlace,
                       foundDate = ui.item.FoundDate,
                       userId = ui.user.Id,
                       userName = ui.user.UserName,
                       Email = ui.user.Email,
                       UserImage = ui.user.AccountPhoto,
                       userPlace = ui.user.City,
                       phoneNumber = ui.user.PhoneNumber,
                       Latitude = ui.item.Latitude,
                       Longitude = ui.item.Longitude,
                       similarity_score = ui.item.similarity_score
                   })
                     .Where(item => descriptions.Contains(item.Description)/* && item.similarity_score >=0.4 */).OrderByDescending(x => x.similarity_score).Take(1).ToList();
             
              

                db.SaveChanges();
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                Log.Debug("{@user} has search for item by description {@descript}", userEmail, model.find_object);
                return Ok(items);
            }
            catch (HttpRequestException ex)
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                Log.Debug("{@user} failed for searching item due to server in the machine Api Server", userEmail);
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("Upload-Images-Search")]
        public IActionResult UploadSearchImage([FromForm]SearchImage model)
        {
            var result = fileRepository.SearchImage(model.Image);

            
            return Ok("The image uploaded");
        }

   
        [HttpGet("get-similar-image-ML")]
        public async Task<IActionResult> GetSimilarImage(string imgurl,int choice)
        {
            var user = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                string SearchUrl = "https://wdw888lb-7075.uks1.devtunnels.ms/search/" + imgurl;

                string baseUrl = "https://32c9-102-189-19-54.ngrok-free.app/";
                
                
                if (choice == 0)
                {

                    string apiUrl = "get_similar_images";

                    string fullUrl = $"{baseUrl}{apiUrl}?img={Uri.EscapeDataString(SearchUrl)}";

                    var httpResponse = await _httpClient.GetAsync(fullUrl);
                    httpResponse.EnsureSuccessStatusCode();

                    var responseData = await httpResponse.Content.ReadAsStringAsync();

                    JObject imgInResponse = JObject.Parse(responseData);

                    var img = imgInResponse["similar_images"]
                                .Select(img => img.ToString()).ToList();

                    var item = db.Item
                        .Join(
                            db.Users,
                            i => i.userId,
                            u => u.Id,
                            (i, u) => new { user = u, item = i }
                            )
                        .Select(ui => new
                        {
                            itemId = ui.item.Id,
                            itemStatus = ui.item.Status,
                            itemImage = ui.item.ImagePhoto,
                            itemName = ui.item.Name,
                            itemAward = ui.item.Award,
                            Description = ui.item.Description,
                            foundPlace = ui.item.FoundPlace,
                            foundDate = ui.item.FoundDate,
                            userId = ui.user.Id,
                            userName = ui.user.UserName,
                            Email = ui.user.Email,
                            UserImage = ui.user.AccountPhoto,
                            userPlace = ui.user.City,
                            phoneNumber = ui.user.PhoneNumber,
                            Latitude = ui.item.Latitude,
                            Longitude = ui.item.Longitude
                        })
                        .Where(i => img.Contains(i.itemImage)).Take(1).ToList();

                    Log.Debug("{@user} is searching for item image ", user.Email );
                    fileRepository.DeleteImageSearch(imgurl);
                    return Ok(item);
                }
                else
                {
                    string apiUrl2 = "find_similar_faces";

                    string fullUrl2 = $"{baseUrl}{apiUrl2}?photo={Uri.EscapeDataString(SearchUrl)}";

                    Log.Debug("{@user} is searching for person image", user.Email);
                    var httpResponse = await _httpClient.GetAsync(fullUrl2);
                    httpResponse.EnsureSuccessStatusCode();

                    var responseData = await httpResponse.Content.ReadAsStringAsync();
                    JObject imgInResponse = JObject.Parse(responseData);

                    var img = imgInResponse["name"]
                                .Select(img => img.ToString()).ToList();

                    var item = db.Item
                     .Join(
                         db.Users,
                         i => i.userId,
                         u => u.Id,
                         (i, u) => new { user = u, item = i }
                         )
                     .Select(ui => new
                     {
                         itemId = ui.item.Id,
                         itemStatus = ui.item.Status,
                         itemImage = ui.item.ImagePhoto,
                         itemName = ui.item.Name,
                         itemAward = ui.item.Award,
                         Description = ui.item.Description,
                         foundPlace = ui.item.FoundPlace,
                         foundDate = ui.item.FoundDate,
                         userId = ui.user.Id,
                         userName = ui.user.UserName,
                         Email = ui.user.Email,
                         UserImage = ui.user.AccountPhoto,
                         userPlace = ui.user.City,
                         phoneNumber = ui.user.PhoneNumber,
                         Latitude = ui.item.Latitude,
                         Longitude = ui.item.Longitude
                     })
                     .Where(i => img.Contains(i.itemImage)).ToList();
                    fileRepository.DeleteImageSearch(imgurl);
                    return Ok(item);
                }


            }
            catch (HttpRequestException ex)
            {
                Log.Debug("{@user} failed to search for item image ", user.Email);
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("add-Index-Ml")]
        public async Task<IActionResult> AddImageIndex([FromBody]List<string> Image)
        {
            try
            {
                var apiUrl = "https://44f2-102-189-104-57.ngrok-free.app/add_images_to_index";

                var response = await _httpClient.PostAsJsonAsync(apiUrl, Image);

                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();

                return Ok(responseData);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.ToString());
            }

        }

        [HttpGet("id-card")]
        public async Task<IActionResult> SearchId(string ImageUrl)
        {
            
            string url = $"id/?photo={ImageUrl}";

            var httpResponse = await _httpClient.GetAsync(url);
            httpResponse.EnsureSuccessStatusCode();

            var responseData = await httpResponse.Content.ReadAsStringAsync();

            var jsonObject = JsonConvert.DeserializeObject<Card>(responseData);

            Card card = new Card()
            {
                id = jsonObject.id,
                frist_name = jsonObject.frist_name,
                rest_of_name = jsonObject.rest_of_name,
                address = jsonObject.address,
                regin = jsonObject.regin,
                birth_date = jsonObject.birth_date,
                birth_place = jsonObject.birth_place,
                gender = jsonObject.gender,
                age = jsonObject.age,
                face = jsonObject.face
            };
            await ItemRepository.AddCard(card);
            return Ok(card);

        }

        [HttpGet("License")]
        public async Task<IActionResult> License(string License)
        {
            string url = $"license/?file={License}";
            var httpResponse = await _httpClient.GetAsync(url);
            httpResponse.EnsureSuccessStatusCode();

            var responseData = await httpResponse.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<Liscence>(responseData);
            Liscence liscence = new Liscence()
            {
                id = jsonObject.id,
                name = jsonObject.name,
                name_English = jsonObject.name_English,
                address = jsonObject.address,
                job = jsonObject.job,
                birth_date = jsonObject.birth_date,
                birth_place = jsonObject.birth_place,
                gender = jsonObject.gender,
                age = jsonObject.age,
                nationality = jsonObject.nationality,
                traffic_unit = jsonObject.traffic_unit,
            };

            db.liscences.Add(liscence);
            db.SaveChanges();
            return Ok(liscence);
        }


    }
}
//https://0735-105-197-94-85.ngrok-free.app/