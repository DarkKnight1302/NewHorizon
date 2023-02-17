using System.ComponentModel.DataAnnotations;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class DeleteYourPostRequest
    {
        [Required]
        public string PostId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string SessionToken { get; set; }
    }
}
