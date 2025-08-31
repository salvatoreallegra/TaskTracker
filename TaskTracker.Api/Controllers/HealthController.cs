using Microsoft.AspNetCore.Mvc;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Root() => Ok(new { ok = true, service = "TaskTracker.Api" });

    [HttpGet("health")]
    public IActionResult Health() => Ok("OK");
}
