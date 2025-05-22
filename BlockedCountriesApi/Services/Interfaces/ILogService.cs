using BlockedCountriesApi.Models;

namespace BlockedCountriesApi.Core.Interfaces
{
    public interface ILogService
    {
        void LogAttempt(BlockedAttemptLog log);
        PaginationResult<IEnumerable<BlockedAttemptLog>> GetLogs(LogsQueryParameter queryParameter);
    }
}
