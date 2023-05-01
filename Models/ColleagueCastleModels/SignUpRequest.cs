using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class SignUpRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 4)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string EmailId { get; set; }

        [Required]
        [EmailAddress]
        public string CorporateEmailId { get; set; }

        [Required]
        public string SignUpToken { get; set; }

        public string PreSignUpToken { get; set; }

        public int ExperienceInYears { get; set; }
    }
}
