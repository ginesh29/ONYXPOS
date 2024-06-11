using System.ComponentModel.DataAnnotations;

namespace Onyx_POS.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = ValidationMessage.REQUIREDVALIDATION)]
        public string UserId { get; set; } = string.Empty;
        [Required(ErrorMessage = ValidationMessage.REQUIREDVALIDATION)]
        public string Password { get; set; } = string.Empty;
    }
}
