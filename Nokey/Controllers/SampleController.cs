using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nokey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetSampleData()
        {
            // Access the User object provided by ControllerBase
            var personId = User.FindFirst("UserId")?.Value;

            // Log the personId (for debugging purposes)
            Console.WriteLine($"This is person id: {personId}");

            // Check if personId is null or empty
            if (string.IsNullOrEmpty(personId))
            {
                return BadRequest("UserId claim not found.");
            }

            // Return the sample data
            return Ok($"Sample data from the sample controller for user ID: {personId}");
        }
    }
}
