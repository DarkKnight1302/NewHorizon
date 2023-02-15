
using NewHorizon.Models.ColleagueCastleModels;

namespace NewHorizon.Repositories.Interfaces
{
    public interface IPropertyPostRepository
    {
        public Task<string> CreatePropertyPostAsync(CreatePropertyObject createPropertyObject);

        public Task<IEnumerable<PropertyPostDetails>> GetAllPropertyPostDetailsAsync(string city, string company);
    }
}
