using Books.Application.Countries.GetCountryList;
using Books.MinimalApi.Endpoints.Internal;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Books.MinimalApi.Endpoints.Countries;

public class GetCountryList : IEndpoint
{
    public static void AddServices(IServiceCollection services,
        IConfiguration configuration)
    {
        // Do nothing, all services were registered in infrastructure layer
    }

    public static void DefineEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{CountryConsts.BaseRoute}", GetCountriesAsync)
            .RequireAuthorization()
            .WithTags(CountryConsts.Tag);
    }

    private static async Task<Ok<IReadOnlyList<CountryResponse>>> GetCountriesAsync(
        ISender sender,
        [FromQuery] string? name,
        CancellationToken token)
    {
        var countries = await sender.Send(new GetCountryListQuery(name), token);
        return TypedResults.Ok(countries.Value);
    }
}
