using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SimRacingDashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataNormalisationController : ControllerBase
    {
        public IActionResult NormaliseF1Export(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
                return BadRequest("No file uploaded.");

            // Process the CSV file here
            // For example, read the file and normalise the data

            // Return a success response
            return Ok("Data normalised successfully.");
        }
    }
}
