namespace BlockedCountriesApi.Models
{
    public class PaginationResult<T>
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public T Data { get; set; }
    }
}
