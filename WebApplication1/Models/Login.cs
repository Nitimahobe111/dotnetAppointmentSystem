using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Login 
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
     //   [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*[\&\$\#\@]).+$", ErrorMessage = "The password must have at least one capital letter and one special character from (&$#@).")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
