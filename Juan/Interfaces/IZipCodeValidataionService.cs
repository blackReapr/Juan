namespace Juan.Interfaces
{
    public interface IZipCodeValidataionService
    {
        Task<bool> ValidateZipCodeAsync(string zipCode);
    }
}