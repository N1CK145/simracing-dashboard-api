using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SimRacingDashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Service is healthy");
        }
    }
}
