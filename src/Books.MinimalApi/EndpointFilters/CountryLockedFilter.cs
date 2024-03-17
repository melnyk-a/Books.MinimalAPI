namespace Books.MinimalApi.EndpointFilters;

public class CountryLockedFilter : IEndpointFilter
{
    private readonly Guid _lockedCountryId;

    public CountryLockedFilter(Guid lockedCountryId)
    {
        _lockedCountryId = lockedCountryId;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        Guid requestCountyId;
        // TODO: Do we need this check?
        if (context.HttpContext.Request.Method == "PUT" ||
            context.HttpContext.Request.Method == "DELETE" ||
            context.HttpContext.Request.Method == "GET")
        {
            requestCountyId = context.GetArgument<Guid>(1);
        }
        else
        {
            throw new NotSupportedException("This filter is not supported for this scenario");
        }

        if (requestCountyId == _lockedCountryId)
        {
            return TypedResults.Problem(new()
            {
                Status = 400,
                Title = "Country was verified and cannot be changed"
            });
        }
        return await next.Invoke(context);
    }
}