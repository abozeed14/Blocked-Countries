using System.ComponentModel.DataAnnotations;

namespace BlockedCountriesApi.Models
{
    public class PaginationParameters
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than or equal to 1.")]
        public int Page { get; set; } = 1;
        
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;
    }
}
