using System.Net;

namespace Books.MinimalApi.EndpointFilters;

public class LogNotFoundResponseFilter : IEndpointFilter
{
    private readonly ILogger<LogNotFoundResponseFilter> _logger;

    public LogNotFoundResponseFilter(ILogger<LogNotFoundResponseFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var result = await next.Invoke(context);

        var actualResult = (result is INestedHttpResult) ?
            ((INestedHttpResult)result).Result :
            (IResult)result;

        if ((actualResult as IStatusCodeHttpResult)?.StatusCode == (int)HttpStatusCode.NotFound)
        {
            _logger.LogInformation("Resource {request} was not found",
                context.HttpContext.Request.Path);
        }

        return result;
    }
}