using Juan.ViewModels;

namespace Juan.Interfaces
{
    public interface ILayoutService
    {
        Task<IEnumerable<CartVM>> GetCartAsync();
        Task UpdateWishlistAsync();
        Task<IDictionary<string, string>> GetSettingsAsync();
    }
}