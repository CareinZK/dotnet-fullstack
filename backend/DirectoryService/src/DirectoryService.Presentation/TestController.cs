using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation;

[ApiController]
[Route("api/[controller]")]
public sealed class TestController : ControllerBase
{
    [HttpPost]
    public IActionResult Post()
    {
        return Ok("Test endpoint is working!");
    }
}
