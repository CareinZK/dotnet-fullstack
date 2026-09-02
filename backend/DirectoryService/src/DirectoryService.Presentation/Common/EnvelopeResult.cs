using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Common;

public class EnvelopeResult : IResult, IActionResult
{
    public object? Envelope { get; }
    public int StatusCode { get; }

    public EnvelopeResult(object? envelope, int statusCode)
    {
        Envelope = envelope;
        StatusCode = statusCode;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = StatusCode;
        if (Envelope is not null)
        {
            await httpContext.Response.WriteAsJsonAsync(Envelope, httpContext.RequestAborted);
        }
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        await ExecuteAsync(context.HttpContext);
    }
}

public sealed class EnvelopeResult<T> : EnvelopeResult
{
    public new Envelope<T> Envelope { get; }

    public EnvelopeResult(Envelope<T> envelope, int statusCode)
        : base(envelope, statusCode)
    {
        Envelope = envelope;
    }
}
