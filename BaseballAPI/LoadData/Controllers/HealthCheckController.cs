using Microsoft.AspNetCore.Mvc;

namespace BaseballAPI.LoadData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> GetHealth() => "Running";

    }
}