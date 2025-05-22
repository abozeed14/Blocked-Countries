using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesApi.Models
{
    public class TemporalBlockRequest
    {
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "CountryCode must be exactly two English capital letters.")]
        public string CountryCode { get; set; }
        [Range(1, 1440 ,ErrorMessage = "Value must be in range of 1 , 1440 minutes")]
        public int DurationMinutes { get; set; }
    }
}
