using LostFindingApi.Models;
using LostFindingApi.Models.DTO;
using LostFindingApi.Models.DTO.AccountDTOs;
using LostFindingApi.Services;
using LostFindingApi.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace LostFindingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtServices _jwtServices;

        private readonly EmailServices _emailServices;
        private readonly IConfiguration _config;
        private readonly IFileRepository fileRepository;
        private readonly ItemRepository itemRepo;
        private readonly ILogger<AccountController> logger;

        public static string Token { get; set; }
        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager,
            JwtServices jwtServices, EmailServices emailServices, IConfiguration config,IFileRepository fileRepository,
            ItemRepository itemRepo,ILogger<AccountController> _logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._jwtServices = jwtServices;
            this._emailServices = emailServices;
            this._config = config;
            this.fileRepository = fileRepository;
            this.itemRepo = itemRepo;
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetToken()
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userid);
            return Ok(user);
        }
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm]RegisterDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            ApiResponse apiResponse = new ApiResponse();
            if (await CheckIfUserExists(model.Email, model.UserName))
            {
                Log.Warning("Email Duplication Error for user {@user}", model.UserName);
                apiResponse.Title = "Email or username Repetition";
                apiResponse.Message = $"Email {model.Email} exists";
                apiResponse.Status = false;
                return Ok(apiResponse);
            }
            if(await CheckIfIdCardExists(model.IdCard))
            {
                Log.Warning("IdCard Duplication for user {@user}", model.UserName);
                apiResponse.Title = "ID Card Repetition";
                apiResponse.Message = $"Id card{model.IdCard} exists";
                apiResponse.Status = false;
                return Ok(apiResponse);
            }
            var addUser = new User();
            if (model.AccountPhoto != null)
            {
                var fileResult = fileRepository.SaveImage(model.AccountPhoto);

                if (fileResult.Item1 == 1)
                {
                    addUser.AccountPhoto = fileResult.Item2;
                }
            }
            
            addUser.Email = model.Email.ToLower();
            addUser.UserName = model.UserName.ToLower();
            addUser.City = model.City.ToLower();
            addUser.region = model.Region.ToLower();
            addUser.PhoneNumber = model.PhoneNumber;
            addUser.idCard = model.IdCard;

            await _userManager.AddToRoleAsync(addUser, SD.UserRole);
            var result = await _userManager.CreateAsync(addUser,model.Password);

            Log.Debug("New user applied on the system with name {@name}", model.UserName);
            if (!result.Succeeded)
            {
                Log.Warning("New user failed to register due to his wrong credentials");
                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                    {
                        apiResponse.Title = "Duplicate username";
                        apiResponse.Message = $"username {model.UserName} is already exists";
                        apiResponse.Status = false;
                        return Ok(apiResponse);
                    }
                  
                }

                return Ok(result.Errors);
            }


            try
            {
                if (await sendTwoFactors(addUser))
                {
                    logger.LogDebug("New user apply after succeeding the credentails");
                    apiResponse.Title = "Registration Succeeded.";
                    apiResponse.Message = "Your registration is successfully done, you can confirm your email.";
                    apiResponse.Status = true;
                    return Ok(apiResponse);
                }
                return BadRequest("Something went wrong,please try later.");
                
            }
            catch(Exception)
            {
                return BadRequest("Failed to send email. Please contact supporter");
            }

        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            ApiResponse apiResponse = new ApiResponse();
            
            if (user == null)
            {
                apiResponse.Title = "Unauthorized access";
                apiResponse.Message = "this user hasn't been registered yet.";
                apiResponse.Status = false;
                return Ok(apiResponse);
            }

            if (user.EmailConfirmed == false)
            {
                apiResponse.Title = "Unauthorized access.";
                apiResponse.Message = "this email hasn't been confirmed it's password yet. please resend email confirmation.";
                apiResponse.Status = false;
                return Ok(apiResponse);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.IsLockedOut)
            {
                return Unauthorized(string.Format("your account had been locked,you should wait until {0} (utc time) to login again", user.LockoutEnd));
            }
            if (!result.Succeeded)
            {
                apiResponse.Title = "Login failed.";
                apiResponse.Message = "Invalid login, password or email is incorrect.";
                apiResponse.Status = false;
                return Ok(apiResponse);
            }



            return await CreateUserDTO(user);
        }

        [HttpGet("refresh-token")]
        public async Task<ActionResult<UserDTO>> RefreshToken()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Email)?.Value);

            return await CreateUserDTO(user);
        }

        [HttpPut("Confirm-email")]
        
        public async Task<ActionResult> ConfirmEmail(ConfirmedEmailDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.username);
            ApiResponse apiResponse = new ApiResponse();
            
            if (user == null)
            {
                apiResponse.Status = false;
                apiResponse.Title = "Unauthorized access";
                apiResponse.Message = $"User {model.username} hasn't been registered yet!";
                return Ok(apiResponse);
            }


            if (user.EmailConfirmed == true)
            {
                apiResponse.Status = false;
                apiResponse.Title = "Two Factor Authentication";
                apiResponse.Message = "You confirmed your email before.";
                return Ok(apiResponse);
            }


            try
            {
                if(model.code == Token)
                {
                    user.EmailConfirmed = true;
                   await _userManager.UpdateAsync(user);
                    apiResponse.Title = "Email confirmation";
                    apiResponse.Message = "Email confirmed successfully";
                    apiResponse.Status = true;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.Status = false;
                    apiResponse.Message = "Your verification code is wrong, please check it or resend anotther one";
                    apiResponse.Title = "Two Factor Authenticaion";

                    return Ok(apiResponse);
                }
              
            }
            catch (Exception)
            {
                return BadRequest("Something went wrong");

            }
        }

        [HttpPost("Resend-email-confirmation/{email}")]
        public async Task<ActionResult> ResendConfirmationEmail(string email)
        {
            if (String.IsNullOrEmpty(email)) { return BadRequest("Invalid email."); }
            var user = await _userManager.FindByEmailAsync(email);

            ApiResponse apiResponse = new ApiResponse();

            if(user == null) {
                apiResponse.Title = "Unauthorized access.";
                apiResponse.Message = "This email has not been register yet";
                apiResponse.Status = true;
                return Ok(apiResponse);
            }
            if(user.EmailConfirmed == true)
            {
                apiResponse.Title = "Email Confirmation Resending.";
                apiResponse.Message = $"Email {user.Email} confirmed before";
                apiResponse.Status = false;
                return Ok(apiResponse);
            }

            try
            {
                if(await sendTwoFactors(user))
                {
                    apiResponse.Title = "Email Confirmation Resending...";
                    apiResponse.Message = $"Email confirmation resend again to {user.Email}.";
                    apiResponse.Status = true;
                    return Ok(apiResponse);
                }
                return BadRequest("Invalid token.");
            }
            catch (Exception)
            {
                return BadRequest("Invalid token");
            }
        }

        [HttpPost("forgot-username-or-password/{email}")]
        public async Task<ActionResult> ForgotPasswordOrUsername(string email)
        {
            if(String.IsNullOrEmpty(email)) { return BadRequest("Invalid email."); }

            var user = await _userManager.FindByEmailAsync(email);
            ApiResponse apiResponse = new ApiResponse();
            if(user == null)
            {
                apiResponse.Title = "Unauthorized access";
                apiResponse.Message = "This email has not been registered yet.";
                apiResponse.Status= false;
                return Ok(apiResponse);
            }
            if (user.EmailConfirmed == false)
            {
                apiResponse.Title = "Confirmation Email";
                apiResponse.Message = "Please confirm your email first or resend your email confirmation. ";
                apiResponse.Status = false;
                return BadRequest(apiResponse); 
            }

            try
            {
                if(await sendForgortPasswordOrUsername(user))
                {
                    apiResponse.Title = "Forgotten Password Email";
                    apiResponse.Message = "Your forgotten password email has been sent";
                    apiResponse.Status = true;
                    return Ok(apiResponse);
                }
                return BadRequest("Failed to send email. Please contact the supporters.");
            }
            catch (Exception)
            {
                return BadRequest("Failed to send email. Please contact the supporters");
            }
        }

        [HttpPut("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDTO model)
        {
            ApiResponse apiResponse = new ApiResponse();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user == null) 
            {
                apiResponse.Status = false;
                apiResponse.Message = "this email hasn't been registered yet.";
                apiResponse.Title = "Unauthorized Access";
                return Ok(apiResponse);
            }
            if(user.EmailConfirmed == false) {
                apiResponse.Title = "Email Confirmation.";
                apiResponse.Message = "You have to confirm your email firstly.";
                apiResponse.Status = false;
                return Ok(apiResponse);
            }
            
            try
            {

                var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
                var decodedTokenString = Encoding.UTF8.GetString(decodedToken);
                var result = await _userManager.ResetPasswordAsync(user, decodedTokenString, model.newPassword);
                if(result.Succeeded) 
                {
                    apiResponse.Title = "Reset Password.";
                    apiResponse.Message = "Reseting password completed successfully.";
                    apiResponse.Status = true;
                    return Ok(apiResponse);
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        apiResponse.Status = false;
                        apiResponse.Message = error.Description;
                        apiResponse.Title = "Reset Password";
                        return Ok(apiResponse);
                    }
                    return Ok();
                }
                
            }
            catch (Exception)
            {
                apiResponse.Message = "Invalid Token";
                apiResponse.Status = false;
                return Ok(apiResponse);
            }
        }

        
        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                itemRepo.deleteUserItems(id);
                fileRepository.DeleteAccountImage(user.AccountPhoto);
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded) 
                {
                    apiResponse.Message = $"Account {user.Email} is deleted.";
                    apiResponse.Status = true;
                    return Ok(apiResponse); 
                }
                apiResponse.Message = "ابوش ايدك اعملي login";
                apiResponse.Status = false;
                return Ok(apiResponse);
            }
            catch(Exception ex)
            {
                return BadRequest($"No user with id {id}");
            }
         
        }


        [HttpGet("Get-Account/{id}")]
        [Authorize]
        public async Task<ActionResult> GetAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return Unauthorized($"user with id {id} not found.");
            }

            var model = new GetUserDTO()
            {
                City = user.City.ToString(),
                Region = user.region.ToString(),
                Email = user.Email.ToString(),
                UserName = user.UserName,
                Phone = user.PhoneNumber,
                AccountPhoto = user.AccountPhoto
            };


            return Ok(model);
        }


        [HttpPut("Update-account")]
        [Authorize]
        public async Task<ActionResult<User>> UpdateAccount([FromForm] UpdateAccountDTO model)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userid);
            ApiResponse apiResponse = new ApiResponse();
            if (user == null)
            {
                
                apiResponse.Message = "ابوس ايدك اعملي login";
                apiResponse.Status = false;
                return Ok(apiResponse);
            }
            if (model.AccountPhoto != null)
                {
                var fileResult = fileRepository.SaveImage(model.AccountPhoto);

                if (fileResult.Item1 == 1)
                {
                    user.AccountPhoto = fileResult.Item2;
                }
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.Phone;
            user.City = model.City;
            user.region = model.Region;
            
            
            var result = await _userManager.UpdateAsync(user);

            apiResponse.Title = "Update Process Succeeded";
            apiResponse.Message = "your account has been updated";
            apiResponse.Status = true;
            return Ok(apiResponse);
        }

        [HttpPut("Change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ChangePaswordDTO model)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userid);

            if(user == null) { return Unauthorized($"No User with id {userid}."); }

            var passwordVerification = await _userManager.CheckPasswordAsync(user, model.OldPassword);

            if (passwordVerification)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Password Change",Message = "Your password successfully changed." }));
                }
                else
                {
                    return BadRequest("Error in the operation. please contact with the supporters.");
                }
            }
            return BadRequest("Invalid password");
        }


        #region Helper Functions
        private async Task<UserDTO> CreateUserDTO(User user)
        {
            return new UserDTO
            {
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Userid = user.Id,
                Title = "Login Successfully done.",
                Message = "Your Login done",
                status = true,
                JWT =await this._jwtServices.CreateJWT(user)
            };
        }

        private async Task<bool> CheckIfUserExists(string email, string userName)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower() || x.UserName == userName.ToLower());
        }

        private async Task<bool> CheckIfIdCardExists(string IDCard)
        {
            return await _userManager.Users.AnyAsync(x => x.idCard == IDCard);
        }
      
        private async Task<bool> sendForgortPasswordOrUsername(User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            
            
            var body = $"<strong>Hello: {user.UserName} </strong>" +
                "<p>You can reset your password by Copying this code to your reset password form </p>" +
                $"<strong>{token}</strong>"+
                "<br><i>Thank you</i>" +
                $"<br> {_config["Email:ApplicationName"]}" +
                $"<br>{DateTime.UtcNow}";

            var emailSend = new EmailSendDTO(user.Email, "Reset your password", body);
            return await _emailServices.SendEmailAsync(emailSend);
        }

        private async Task<bool> sendTwoFactors(User user)
        {
            Token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            var body = $"<strong>Hello : {user.UserName}" +
                $"<br>your verification code is {Token}";

            var emailSend = new EmailSendDTO(user.Email, "OTP", body);
            return await _emailServices.SendEmailAsync(emailSend);
        }
        #endregion
    }
}
