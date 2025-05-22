using BlockedCountriesApi.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace BlockedCountriesApi.BackgroundServices
{
    public class TemporalBlockCleanupService : BackgroundService
    {
        private readonly IBlockService _blockService;
        private readonly ILogger<TemporalBlockCleanupService> _logger;
        
        public TemporalBlockCleanupService(IBlockService blockService, ILogger<TemporalBlockCleanupService> logger)
        {
            _blockService = blockService;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _blockService.CleanupExpiredBlocks();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up expired blocks");
                }
                
                // Wait for the next cleanup interval
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            
        }
    }
}
