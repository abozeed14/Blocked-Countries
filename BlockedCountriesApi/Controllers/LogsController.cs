using BlockedCountriesApi.Core.Interfaces;
using BlockedCountriesApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogService logService, ILogger<LogsController> logger)
        {
            _logService = logService;
            _logger = logger;
        }

        [HttpGet("blocked-attempts")]
        public IActionResult GetLogs([FromQuery]LogsQueryParameter query)
        {
            try
            {
                // Validate pagination parameters
                if (query.Page < 1)
                {
                    return BadRequest("Page number must be greater than or equal to 1.");
                }

                if (query.PageSize < 1 || query.PageSize > 100)
                {
                    return BadRequest("Page size must be between 1 and 100.");
                }

                var logs = _logService.GetLogs(query);

                return Ok(new
                {
                    Total = logs.TotalCount,
                    TotalPages = logs.TotalPages,
                    Page = query.Page,
                    Data = logs.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving logs.");
                return StatusCode(500, "An error occurred while retrieving logs.");
            }
        }
    }

}
