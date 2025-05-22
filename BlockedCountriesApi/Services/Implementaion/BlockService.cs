using BlockedCountriesApi.Core.Interfaces;
using BlockedCountriesApi.Models;
using System.Collections.Concurrent;

namespace BlockedCountriesApi.Services.Implementaion
{
    public class BlockService : IBlockService
    {
        private readonly ConcurrentDictionary<string, DateTime?> _blocked = new();

        public bool AddBlock(string code) => _blocked.TryAdd(code.ToUpper(), null);
        public bool RemoveBlock(string code) => _blocked.TryRemove(code.ToUpper(), out _);
        public bool IsBlocked(string code) => _blocked.TryGetValue(code.ToUpper(), out var expires) && (!expires.HasValue || expires > DateTime.UtcNow);
        public PaginationResult<IEnumerable<KeyValuePair<string, DateTime?>>> GetAllBlocks(BlockedCountriesQueryParameters queryParameters)
        {
            // Validate query parameters
            int page = queryParameters.Page > 0 ? queryParameters.Page : 1;
            int pageSize = queryParameters.PageSize > 0 ? queryParameters.PageSize : 10;

            // Filter and sort the data
            var filtered = _blocked
                .Where(c => string.IsNullOrWhiteSpace(queryParameters.CountryCode) ||
                            c.Key.Contains(queryParameters.CountryCode, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Key)
                .ToList(); 

            // Calculate pagination details
            int totalCount = filtered.Count;
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

            // Apply pagination
            var data = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            // Return the result
            return new PaginationResult<IEnumerable<KeyValuePair<string, DateTime?>>>
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = data
            };
        }
        public bool AddTemporalBlock(string code, DateTime expiresAt) => _blocked.TryAdd(code.ToUpper(), expiresAt);
        public bool IsTemporalBlock(string code) => _blocked.Any(c=>c.Key==code && c.Value.HasValue);

        public void CleanupExpiredBlocks()
        {
            foreach (var kvp in _blocked)
            {
                if (kvp.Value.HasValue && kvp.Value.Value <= DateTime.Now)
                    _blocked.TryRemove(kvp.Key, out _);
            }
        }
    }
}
