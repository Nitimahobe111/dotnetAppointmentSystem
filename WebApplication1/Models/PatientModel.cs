using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace WebApplication1.Models
{
    public class PatientModel : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        public string? Gender { get; set; }

        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "The phone number must be exactly 10 digits.")]
        public string? PhoneNumber { get; set; } // Changed to string for easier validation

        [Display(Name = "Country Code")]
        [RegularExpression(@"^\+\d{1,4}$", ErrorMessage = "Please enter a valid country code.")]
        public string? CountryCode { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*[\&\$\#\@]).+$", ErrorMessage = "The password must have at least one capital letter and one special character from (&$#@).")]
        public string? PassWord { get; set; }

        [DataType(DataType.Password)]
        [Compare("PassWord", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassWord { get; set; }

        [StringLength(500, MinimumLength = 75, ErrorMessage = "The description must be at least 75 characters long.")]
        public string? Description { get; set; }
        public bool? IsActive { get; set; } = true;
        public byte[]? ProfilePicture { get; set; }
        [NotMapped]
        public IFormFile? pic {  get; set; }
    }
}
