using NewHorizon.Models.ColleagueCastleModels;

namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface IInterestService
    {
        public void ShowInterestInPost(string postId, string UserId);

        public Task<IEnumerable<PropertyPostResponse>> FindShortlistedProperties(string UserId); 
    }
}
