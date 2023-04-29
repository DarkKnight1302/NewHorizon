using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Spatial;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.Interfaces;
using NewHorizon.Utils;

namespace NewHorizon.Repositories
{
    public class PropertyPostRepository : IPropertyPostRepository
    {
        private readonly Container container;
        private readonly Container PropertyDetailsContainer;
        private readonly Container PropertyDetailsArchiveContainer;

        public PropertyPostRepository(ICosmosDbService cosmosDbService)
        {
            this.container = cosmosDbService.GetContainerFromColleagueCastle("PropertyPost");
            this.PropertyDetailsContainer = cosmosDbService.GetContainerFromColleagueCastle("PropertyPostDetails");
            this.PropertyDetailsArchiveContainer = cosmosDbService.GetContainerFromColleagueCastle("PropertyPostDetailsArchive");
        }

        public async Task<string> CreatePropertyPostAsync(CreatePropertyObject createPropertyObject)
        {
            string uniqueId = this.GetUniqueIdForPost(createPropertyObject.username, createPropertyObject.placeId);
            var propertyPost = new PropertyPost
            {
                Id = uniqueId,
                Uid = uniqueId,
                Location = createPropertyObject.location,
                PlaceId = createPropertyObject.placeId,
                Available = true,
                City = createPropertyObject.city,
                Company = createPropertyObject.company,
                CreatedAt = DateTime.UtcNow
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
                RentAmount = createPropertyObject.RentAmount,
                TenantPreference = createPropertyObject.TenantPreference,
                FoodPreference = createPropertyObject.FoodPreference,
                ExperienceRange = createPropertyObject.ExperienceRange,
                PropertyType = createPropertyObject.PropertyType,
                FurnishingType = createPropertyObject.FurnishingType,
                FlatType = createPropertyObject.FlatType,
                Drinking = createPropertyObject.Drinking,
                Smoking = createPropertyObject.Smoking,
                Location = createPropertyObject.location,
            };
            await this.PropertyDetailsContainer.UpsertItemAsync(propertyPostDetails).ConfigureAwait(false);
            return uniqueId;
        }

        public async Task<bool> DeletePropertyPostAsync(PropertyPostDetails propertyPostDetails)
        {
            PropertyPost propertyPost = await this.container.ReadItemAsync<PropertyPost>(propertyPostDetails.Id, new PartitionKey(propertyPostDetails.Id)).ConfigureAwait(false);

            if (propertyPost == null)
            {
                return false;
            }
            string uid = Guid.NewGuid().ToString();
            PropertyPostDetailsArchive postDetailsArchive = new PropertyPostDetailsArchive()
            {
                Id = uid,
                Uid = uid,
                PostId = propertyPostDetails.Id,
                PropertyPostDetails = propertyPostDetails,
                PropertyPost = propertyPost,
                CreatedAt = DateTime.UtcNow,
            };
            await this.PropertyDetailsArchiveContainer.CreateItemAsync(postDetailsArchive).ConfigureAwait(false);
            await this.container.DeleteItemAsync<PropertyPost>(propertyPostDetails.Id, new PartitionKey(propertyPostDetails.Id)).ConfigureAwait(false);
            await this.PropertyDetailsContainer.DeleteItemAsync<PropertyPostDetails>(propertyPostDetails.Id, new PartitionKey(propertyPostDetails.Id)).ConfigureAwait(false);
            return true;
        }

        public async Task<IEnumerable<PropertyPostDetails>> GetAllAvailablePostOfUserAsync(string userId)
        {
            QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.CreatorUserName = @value1")
            .WithParameter("@value1", userId);

            List<PropertyPostDetails> propertyPostDetails = new List<PropertyPostDetails>();

            // Execute the query and retrieve the results
            FeedIterator<PropertyPostDetails> queryResultSet = PropertyDetailsContainer.GetItemQueryIterator<PropertyPostDetails>(queryDefinition);
            while (queryResultSet.HasMoreResults)
            {
                FeedResponse<PropertyPostDetails> currentResultSet = await queryResultSet.ReadNextAsync().ConfigureAwait(false);
                if (currentResultSet != null && currentResultSet.Count > 0)
                {
                    propertyPostDetails.AddRange(currentResultSet.Resource);
                }
            }
            return propertyPostDetails;
        }

        public async Task<IEnumerable<PropertyPostDetails>> GetAllPropertyPostDetailsAsync(Location location, string company, int radialDistance)
        {
            var center = new Point(location.coordinates[0], location.coordinates[1]);
            QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.Company = @value2 and c.Available = @value3 and ST_DISTANCE(c.Location, @center) < @radius")
            .WithParameter("@value2", company).WithParameter("@value3", true).WithParameter("@center", center).WithParameter("@radius", radialDistance * 1000);

            List<(string, PartitionKey)> propertyPostIds = new List<(string, PartitionKey)>();

            // Execute the query and retrieve the results
            FeedIterator<PropertyPost> queryResultSet = container.GetItemQueryIterator<PropertyPost>(queryDefinition);
            while (queryResultSet.HasMoreResults)
            {
                FeedResponse<PropertyPost> currentResultSet = await queryResultSet.ReadNextAsync().ConfigureAwait(false);
                if (currentResultSet != null && currentResultSet.Count > 0)
                {
                    foreach (var post in currentResultSet.Resource)
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

        public async Task<PropertyPostDetails> GetPropertryPostDetailsById(string postId)
        {
            var itemResponse = await this.PropertyDetailsContainer.ReadItemAsync<PropertyPostDetails>(postId, new PartitionKey(postId)).ConfigureAwait(false);
            if (itemResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return itemResponse.Resource;
            }
            return null;
        }

        private string GetUniqueIdForPost(string username, string placeId)
        {
            return username + "_" + placeId;
        }
    }
}
