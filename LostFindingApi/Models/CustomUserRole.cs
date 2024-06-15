using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models
{
    public class CustomUserRole : IdentityUserRole<string>
    {
        [Key]
        public override string UserId { get; set; }
        [Key]
        public override string RoleId { get; set; }
    }
}
