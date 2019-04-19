using Microsoft.AspNetCore.Mvc;

namespace EggPlantApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetVersion()
        {
            return Ok("Version1");
        }
    }
}