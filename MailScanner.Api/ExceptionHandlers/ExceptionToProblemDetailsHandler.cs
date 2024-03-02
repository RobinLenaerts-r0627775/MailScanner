

namespace MailScanner.Api;

public class ExceptionToProblemDetailsHandler : Microsoft.AspNetCore.Diagnostics.IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = "An error occurred",
            Detail = exception.Message,
            Type = exception.GetType().Name,
            Status = (int)HttpStatusCode.BadRequest
        }, cancellationToken: cancellationToken);

        return true;
    }
}