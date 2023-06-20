using Microsoft.AspNetCore.Mvc;

namespace Supreme.Api.Controllers;

[Route("readiness")]
public class ReadinessController : ControllerBase
{
    public ReadinessController()
    {
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return StatusCode(StatusCodes.Status200OK, new
        {
            ServerTime = DateTime.Now
        });
    }
}
