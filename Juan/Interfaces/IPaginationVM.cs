namespace Juan.Interfaces
{
    public interface IPaginationVM
    {
        string CategoryName { get; }
        string ColorName { get; }
        int CurrentPage { get; }
        int End { get; }
        bool HasNext { get; }
        bool HasPrevious { get; }
        int PageCount { get; }
        string PriceRange { get; }
        string SizeName { get; }
        string SortBy { get; }
        int Start { get; }
        int ItemsCount { get; }
        int Take { get; }
    }
}