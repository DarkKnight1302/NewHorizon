using Microsoft.Azure.Cosmos;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Repositories
{
    public class PropertyPostRepository : IPropertyPostRepository
    {
        private readonly Container container;
        private readonly Container PropertyDetailsContainer;

        public PropertyPostRepository(ICosmosDbService cosmosDbService)
        {
            this.container = cosmosDbService.GetContainerFromColleagueCastle("PropertyPost");
            this.PropertyDetailsContainer = cosmosDbService.GetContainerFromColleagueCastle("PropertyPostDetails");
        }

        public async Task<string> CreatePropertyPostAsync(CreatePropertyObject createPropertyObject)
        {
            string uniqueId = this.GetUniqueIdForPost(createPropertyObject.username, createPropertyObject.placeId);
            var propertyPost = new PropertyPost
            {
                Id = uniqueId,
                Uid = uniqueId,
                PlaceId = createPropertyObject.placeId,
                Location = createPropertyObject.location,
                Available = true,
                City = createPropertyObject.city,
                Company = createPropertyObject.company,
            };
            await this.container.UpsertItemAsync(propertyPost).ConfigureAwait(false);
            var propertyPostDetails = new PropertyPostDetails
            {
                Id = uniqueId,
                Uid = uniqueId,
                CreatorUserName = createPropertyObject.username,
                Title = createPropertyObject.title,
                Description = createPropertyObject.description,
                FormattedAddress = createPropertyObject.FormattedAddress,
                Images = createPropertyObject.Images,
                MapUrl = createPropertyObject.MapUrl,
            };
            await this.PropertyDetailsContainer.UpsertItemAsync(propertyPostDetails).ConfigureAwait(false);
            return uniqueId;
        }

        private string GetUniqueIdForPost(string username, string placeId)
        {
            return username + "_" + placeId;
        }
    }
}
