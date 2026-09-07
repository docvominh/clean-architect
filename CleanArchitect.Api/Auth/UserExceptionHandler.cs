using CleanArchitect.Application.UserAggregate;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitect.Api.Auth;

// Maps UserAggregate domain exceptions to HTTP responses so command handlers can just throw,
// instead of every controller action repeating the same try/catch.
public sealed class UserExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case UserValidationException validation:
                var problem = new HttpValidationProblemDetails(new Dictionary<string, string[]>(validation.Errors))
                {
                    Status = StatusCodes.Status400BadRequest
                };
                httpContext.Response.StatusCode = problem.Status.Value;
                await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
                return true;

            case UserAuthenticationException:
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return true;

            default:
                return false;
        }
    }
}
