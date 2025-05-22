using BlockedCountriesApi.Models;

namespace BlockedCountriesApi.Core.Interfaces
{
    public interface IBlockService
    {
        bool AddBlock(string code);
        bool RemoveBlock(string code);
        bool IsBlocked(string code);
        PaginationResult<IEnumerable<KeyValuePair<string, DateTime?>>> GetAllBlocks(BlockedCountriesQueryParameters queryParameters);
        bool AddTemporalBlock(string code, DateTime expiresAt);
        bool IsTemporalBlock(string code);
        void CleanupExpiredBlocks();
    }
}
