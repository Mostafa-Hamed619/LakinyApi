using Azure.Identity;
using LostFindingApi.Models;
using LostFindingApi.Models.Data;
using LostFindingApi.Models.DTO.AdminDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LostFindingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles ="Admin")]
    public class AdminController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        private readonly DataContext db;

        public AdminController(RoleManager<IdentityRole> roleManager,UserManager<User> userManager,DataContext db)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.db = db;
        }

        [HttpPost("Add-Role")]
        public async Task<IActionResult> AddRole([FromBody]AddRoleDTO model)
        {
            var Role = new IdentityRole()
            {
                Name = model.roleName
            };

            var result = await roleManager.CreateAsync(Role);
            if (result.Succeeded)
            {
                return Ok(new JsonResult(new { tile = "Role Addition", message = $"role {model.roleName} is successfully added" }));
            }
            return BadRequest("Error in addition process");
        }

        [HttpDelete("Delete-Role/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return NotFound($"No role with id {id} exists.");
            }
            return Ok(new JsonResult(new
            {
                title = "Delete process",
                Message = $"role id {role.Name} is deleted successfully."
            }));
        }

        [HttpGet("Get-Role-Details")]
        public async Task<ActionResult<IEnumerable<object>>> GetRoleDetails()
        {
            var result = await db.Roles
                .Join(
                        db.UserRoles,
                        role => role.Id,
                        userRole => userRole.RoleId,
                        (role, userRole) => new { r = role, ur = userRole }
                ).Join(
                        db.Users,
                        urId => urId.ur.UserId,
                        user => user.Id,
                        (userRole, user) => new { ur = userRole, u = user }
                ).Select(ur => new { UserName = ur.u.UserName,Email = ur.u.Email,
                    Place = ur.u.City+ ", " + ur.u.region, Role = ur.ur.r.Name,userId = ur.u.Id }).ToListAsync();

            return Ok(result);
        }

        [HttpGet("Get-All-Users")]
       
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var user = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var Result = await userManager.Users.Select(u => new {
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    City = u.City,
                    Region = u.region,
                    DateCreated = u.DateCreated,
                    userId = u.Id,
                    AccountPhoto = u.AccountPhoto
                }).ToListAsync();

                if (Result.Count == 0)
                {
                    return Ok("No Users has registered yet.");
                }
                else
                {
                    return Ok(Result);
                }
            
           
           
        }

        [HttpDelete("Delet-User")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            await userManager.DeleteAsync(user);

            return Ok("User Deleted");
        }

        [HttpPut("lock-Member/{id}")]
        public async Task<IActionResult> LockMember(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            if(IsAdminUserId(user.Id))
            {
                return BadRequest("you can not lock admin. ");
            }

            await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(5));
            return Ok($"{user.UserName} has been blocked");
        }

        [HttpPut("unlock-member/{id}")]
        public async Task<IActionResult> UnlockMember(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            if (IsAdminUserId(user.Id))
            {
                return BadRequest("you can not lock admin. ");
            }

            await userManager.SetLockoutEndDateAsync(user, null);
            return Ok($"user {user.Email} had been unlocked.");
        }


        private bool IsAdminUserId(string userId)
        {
            return userManager.FindByIdAsync(userId).GetAwaiter().GetResult().UserName.Equals(SD.AdminUserName);
        }
    }
}
