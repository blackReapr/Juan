using Juan.ViewModels;

namespace Juan.Interfaces
{
    public interface ILayoutService
    {
        Task<IEnumerable<CartVM>> GetCartAsync();
    }
}