using Juan.Interfaces;
using Newtonsoft.Json.Linq;

namespace Juan.Services;

public class ZipCodeValidataionService : IZipCodeValidataionService
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "e42420a0-4c14-11ef-8de3-a960b96bd17a";

    public ZipCodeValidataionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ValidateZipCodeAsync(string zipCode)
    {
        var url = $"https://app.zipcodebase.com/api/v1/search?apikey={ApiKey}&codes={zipCode}";

        HttpResponseMessage response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        string responseBody = await response.Content.ReadAsStringAsync();
        JObject json = JObject.Parse(responseBody);
        var query = json["results"];
        return query.HasValues;
    }
}
