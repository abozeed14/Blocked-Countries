using BlockedCountriesApi.Core.Interfaces;
using BlockedCountriesApi.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BlockedCountriesApi.Core.Services
{
    public class GeoService : IGeoService
    {
        private readonly HttpClient _client;
        private readonly GeoSettings _settings;

        public GeoService(HttpClient client, IOptions<GeoSettings> settings)
        {
            _client = client;
            _settings = settings.Value;
        }

        public async Task<(bool Success, string CountryCode, string CountryName)> LookupCountryAsync(string ip)
        {
            var apiKey = _settings.ApiKey;
            var baseUrl = _settings.BaseUrl;

            var url = string.IsNullOrWhiteSpace(ip)
                ? $"{baseUrl}?apiKey={apiKey}"
                : $"{baseUrl}?apiKey={apiKey}&ip={ip}";

            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return (false, null, null);

            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);
            return (true, json.location.country_code2, json.location.country_name);
        }

    }

}
