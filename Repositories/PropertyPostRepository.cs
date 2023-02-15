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
                Location = createPropertyObject.location,
                MapUrl = createPropertyObject.MapUrl,
                RentAmount = createPropertyObject.RentAmount,
                TenantPreference = createPropertyObject.TenantPreference,
                FoodPreference = createPropertyObject.FoodPreference,
                ExperienceRange = createPropertyObject.ExperienceRange,
                PropertyType = createPropertyObject.PropertyType,
                FurnishingType = createPropertyObject.FurnishingType,
                FlatType = createPropertyObject.FlatType,
                Drinking = createPropertyObject.Drinking,
                Smoking = createPropertyObject.Smoking,
            };
            await this.PropertyDetailsContainer.UpsertItemAsync(propertyPostDetails).ConfigureAwait(false);
            return uniqueId;
        }

        public async Task<IEnumerable<PropertyPostDetails>> GetAllPropertyPostDetailsAsync(string city, string company)
        {
            QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.City = @value1 and c.Company = @value2")
            .WithParameter("@value1", city).WithParameter("@value2", company);

            List<(string, PartitionKey)> propertyPostIds = new List<(string, PartitionKey)>();

            // Execute the query and retrieve the results
            FeedIterator<PropertyPost> queryResultSet = container.GetItemQueryIterator<PropertyPost>(queryDefinition);
            while (queryResultSet.HasMoreResults)
            {
                FeedResponse<PropertyPost> currentResultSet = await queryResultSet.ReadNextAsync().ConfigureAwait(false);
                if (currentResultSet != null && currentResultSet.Count > 0)
                {
                    foreach(var post in currentResultSet.Resource)
                    {
                        propertyPostIds.Add((post.Id, new PartitionKey(post.Id)));
                    }
                }
            }
            if (propertyPostIds.Count > 0)
            {
                FeedResponse<PropertyPostDetails> propertyPostResponse = await this.PropertyDetailsContainer.ReadManyItemsAsync<PropertyPostDetails>(propertyPostIds).ConfigureAwait(false);
                if (propertyPostResponse != null && propertyPostResponse.Resource.Any())
                {
                    return propertyPostResponse.Resource;
                }
            }
           return Enumerable.Empty<PropertyPostDetails>();
        }

        private string GetUniqueIdForPost(string username, string placeId)
        {
            return username + "_" + placeId;
        }
    }
}
