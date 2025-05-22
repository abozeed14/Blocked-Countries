using BlockedCountriesApi.Core.Interfaces;
using BlockedCountriesApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly IBlockService _blockService;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(IBlockService blockService, ILogger<CountriesController> logger)
        {
            _blockService = blockService;
            _logger = logger;
        }

        [HttpPost("block")]
        public IActionResult BlockCountry([FromBody] CountryBlockRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(request.CountryCode))
                {
                    return BadRequest("Country code is required.");
                }

                if (!_blockService.AddBlock(request.CountryCode))
                    return Conflict("Country already blocked.");
                
                _logger.LogDebug($"Country {request.CountryCode} blocked succesfully");
                return Ok("Country blocked succesfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error occurred while processing your request.");
                return StatusCode(500, "An Error occurred while processing your request.");
            }
        }

        [HttpDelete("block/{countryCode}")]
        public IActionResult UnblockCountry(string countryCode)
        {
            try
            {
                if (string.IsNullOrEmpty(countryCode))
                {
                    return BadRequest("Country code is required.");
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(countryCode, @"^[A-Z]{2}$"))
                {
                    return BadRequest("Country code must be exactly two English capital letters.");
                }

                if (!_blockService.RemoveBlock(countryCode))
                    return NotFound("Country not found.");

                return Ok("Country unblocked succesfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error occurred while processing your request.");
                return StatusCode(500, "An Error occurred while processing your request.");
            }
        }

        [HttpGet("blocked")]
        public IActionResult GetBlockedCountries([FromQuery] BlockedCountriesQueryParameters query)
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

                // Validate country code if provided
                if (!string.IsNullOrEmpty(query.CountryCode) && 
                    !System.Text.RegularExpressions.Regex.IsMatch(query.CountryCode, @"^[A-Z]{2}$"))
                {
                    return BadRequest("Country code must be exactly two English capital letters.");
                }

                var blocked = _blockService.GetAllBlocks(query);
                return Ok(new
                {
                    TotalCount = blocked.TotalCount,
                    TotalPages = blocked.TotalPages,
                    PageNumber = query.Page,
                    Data = blocked.Data.Select(x => new { CountryCode = x.Key, Expiration = x.Value })
                });
          }
          catch (Exception ex)
          {
                _logger.LogError(ex, "An Error occurred while processing your request.");
                return StatusCode(500, "An Error occurred while processing your request.");
          }
        }

        [HttpPost("temporal-block")]
        public IActionResult TemporalBlock([FromBody] TemporalBlockRequest request)
        {
           try
           {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(request.CountryCode))
                {
                    return BadRequest("Country code is required.");
                }

                if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
                    return BadRequest("Duration must be between 1 and 1440 minutes.");

                if (_blockService.IsTemporalBlock(request.CountryCode))
                    return Conflict("Country already temporarily blocked.");

                // Fix: Use the actual duration instead of current time
                var expiresAt = DateTime.Now.AddMinutes(request.DurationMinutes);

                if (!_blockService.AddTemporalBlock(request.CountryCode, expiresAt))
                    return Conflict("Country already blocked.");

                return Ok("Country blocked succesfully");
           }
           catch (Exception ex)
           {
                _logger.LogError(ex, "An Error occurred while processing your request.");
                return StatusCode(500, "An Error occurred while processing your request.");
           }
        }
    }
}
