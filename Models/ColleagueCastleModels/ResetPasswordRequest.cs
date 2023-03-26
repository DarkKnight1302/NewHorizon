using System.ComponentModel.DataAnnotations;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
