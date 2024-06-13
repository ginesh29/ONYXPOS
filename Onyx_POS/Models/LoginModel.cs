using System.ComponentModel.DataAnnotations;

namespace Onyx_POS.Models
{
    public class LoginModel
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
