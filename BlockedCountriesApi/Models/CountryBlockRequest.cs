using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesApi.Models
{
    public class CountryBlockRequest
    {
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "CountryCode must be exactly two English capital letters.")]
        public string CountryCode { get; set; }
    }
}
