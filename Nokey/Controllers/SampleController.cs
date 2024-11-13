using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web_Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        [Authorize(Roles ="User")]
        [HttpGet]
        public async Task<string> GetSampleData()
        {
            return "sample data from the sample controller";
        }
    }
}
