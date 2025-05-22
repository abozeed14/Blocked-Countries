using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesApi.Models
{
    public class BlockedCountriesQueryParameters : PaginationParameters
    {
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "CountryCode must be exactly two English capital letters.")]
        public string? CountryCode { get; set; }
    }
}
