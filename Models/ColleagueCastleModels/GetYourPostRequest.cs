using System.ComponentModel.DataAnnotations;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class GetYourPostRequest
    {
        [Required]
        public string SessionId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
