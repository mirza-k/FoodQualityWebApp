using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Services.Interface;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IQualityManagerService _qualityManagerService;
        public FoodController(IQualityManagerService qualityManagerService)
        {
            _qualityManagerService = qualityManagerService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] ProcessFoodRequest processFoodRequest)
        {
            var response = await _qualityManagerService.Process(processFoodRequest);

            return Ok(response);
        }

        [HttpGet("status/{serial_number}")]
        public async Task<IActionResult> GetStatusBySerialNumber(string serial_number)
        {
            if (string.IsNullOrWhiteSpace(serial_number))
            {
                return BadRequest("Serial number is required.");
            }

            var response = await _qualityManagerService.GetStateBySerialNumber(serial_number);

            return Ok(response);
        }
    }
}