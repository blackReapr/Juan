using Juan.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Juan.ViewModels;

public class PaginationVM<T> : List<T>, IPaginationVM
{
    public PaginationVM(IEnumerable<T> items, int page, int count, string? categoryName = null, string? colorName = null, string? sizeName = null, string? priceRange = null, int itemsCount = 0, int take = 0)
    {
        CurrentPage = page;
        PageCount = count;
        Start = CurrentPage - 2 <= 0 ? 1 : CurrentPage + 2 >= PageCount ? PageCount - 4 >= 1 ? PageCount - 4 : 1 : CurrentPage - 2;
        End = CurrentPage + 2 > PageCount ? PageCount : CurrentPage - 2 <= 0 ? 5 : CurrentPage + 2;
        CategoryName = categoryName ?? string.Empty;
        ColorName = colorName ?? string.Empty;
        SizeName = sizeName ?? string.Empty;
        PriceRange = priceRange ?? string.Empty;
        ItemsCount = itemsCount;
        Take = take;
        AddRange(items);
    }

    public int CurrentPage { get; }
    public int ItemsCount { get; }
    public int PageCount { get; }
    public int Start { get; }
    public int Take { get; }
    public int End { get; }
    public bool HasNext => CurrentPage < PageCount;
    public bool HasPrevious => CurrentPage > 1;
    public string CategoryName { get; }
    public string ColorName { get; }
    public string SizeName { get; }
    public string PriceRange { get; }

    public static async Task<PaginationVM<T>> CreateAsync(IQueryable<T> query, int page, int take = 3, string? categoryName = null, string? colorName = null, string? sizeName = null, string? priceRange = null)
    {
        int count = (int)Math.Ceiling((decimal)query.Count() / take);
        IEnumerable<T> items = await query.Skip((page - 1) * take).Take(take).ToListAsync();
        return new PaginationVM<T>(items, page, count, categoryName, colorName, sizeName, priceRange, query.Count(), take);
    }
}
