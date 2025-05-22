using BlockedCountriesApi.Core.Interfaces;
using BlockedCountriesApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IpController : ControllerBase
    {
        private readonly IGeoService _geoService;
        private readonly IBlockService _blockService;
        private readonly ILogService _logService;
        private readonly ILogger<IpController> _logger;

        public IpController(IGeoService geoService, IBlockService blockService, ILogService logService, ILogger<IpController> logger)
        {
            _geoService = geoService;
            _blockService = blockService;
            _logService = logService;
            _logger = logger;
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string? ipAddress)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                ipAddress ??= ip;

                // Validate IP address is not null or empty
                if (string.IsNullOrEmpty(ipAddress))
                {
                    return BadRequest("IP address is required but could not be determined from request.");
                }

                // Validate IP address format
                if (!System.Net.IPAddress.TryParse(ipAddress, out _))
                {
                    return BadRequest("Invalid IP address format.");
                }

                var result = await _geoService.LookupCountryAsync(ipAddress);

                if (!result.Success) 
                {
                    return BadRequest("IP lookup failed. The geolocation service may be unavailable.");
                }

                return Ok(new { result.CountryCode, result.CountryName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error occurred while processing your request.");
                return StatusCode(500, "An Error occurred while processing your request.");
            }
        }

        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock()
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                
                // Validate IP address is not null or empty
                if (string.IsNullOrEmpty(ip))
                {
                    return BadRequest("Client IP address could not be determined.");
                }

                var userAgent = Request.Headers["User-Agent"].ToString();

                var result = await _geoService.LookupCountryAsync(ip);

                // Check if geolocation lookup was successful
                if (!result.Success)
                {
                    return BadRequest("IP geolocation lookup failed. The service may be unavailable.");
                }

                // Validate country code from result
                if (string.IsNullOrEmpty(result.CountryCode))
                {
                    return BadRequest("Could not determine country from IP address.");
                }

                var isBlocked = _blockService.IsBlocked(result.CountryCode);

                _logService.LogAttempt(new BlockedAttemptLog
                {
                    IpAddress = ip,
                    CountryCode = result.CountryCode,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow,
                    IsBlocked = isBlocked
                });

                return Ok(new { IsBlocked = isBlocked, Ip = ip, CountryCode = result.CountryCode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error occurred while processing your request.");
                return StatusCode(500, "An Error occurred while processing your request.");
            }
        }
    }
}
