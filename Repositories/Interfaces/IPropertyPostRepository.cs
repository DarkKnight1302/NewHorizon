
using NewHorizon.Models.ColleagueCastleModels;

namespace NewHorizon.Repositories.Interfaces
{
    public interface IPropertyPostRepository
    {
        public Task<string> CreatePropertyPostAsync(CreatePropertyObject createPropertyObject);
    }
}
