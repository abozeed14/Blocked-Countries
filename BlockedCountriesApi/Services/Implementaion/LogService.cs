using BlockedCountriesApi.Core.Interfaces;
using BlockedCountriesApi.Models;

namespace BlockedCountriesApi.Core.Services
{
    public class LogService : ILogService
    {
        private readonly List<BlockedAttemptLog> _logs = new();
        public void LogAttempt(BlockedAttemptLog log)
        {
            lock (_logs) { _logs.Add(log); }
        }


        PaginationResult<IEnumerable<BlockedAttemptLog>> ILogService.GetLogs(LogsQueryParameter queryParameters)
        {
            // Validate query parameters
            int page = queryParameters.Page > 0 ? queryParameters.Page : 1;
            int pageSize = queryParameters.PageSize > 0 ? queryParameters.PageSize : 10;


            var sorted = _logs.OrderByDescending(l => l.Timestamp);
            // Calculate pagination details
            int totalCount = sorted.Count();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            
            var data = sorted
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .ToList();
            return new PaginationResult<IEnumerable<BlockedAttemptLog>>
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = data
            };
        }
    }
}
