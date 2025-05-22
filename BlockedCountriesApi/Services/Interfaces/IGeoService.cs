namespace BlockedCountriesApi.Core.Interfaces
{
    public interface IGeoService
    {
        Task<(bool Success, string CountryCode, string CountryName)> LookupCountryAsync(string ip);

    }
}
