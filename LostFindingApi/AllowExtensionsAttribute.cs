using System.ComponentModel.DataAnnotations;

namespace LostFindingApi
{
    public class AllowExtensionsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return base.IsValid(value, validationContext);
        }
    }
}
