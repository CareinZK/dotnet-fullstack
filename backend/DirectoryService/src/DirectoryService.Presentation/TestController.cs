using DirectoryService.Presentation.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
public sealed class TestController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Envelope<string>), StatusCodes.Status200OK)]
    public IResult Post()
    {
        return new EnvelopeResult<string>(Envelope<string>.Ok("Test endpoint is working!"), StatusCodes.Status200OK);
    }
}
