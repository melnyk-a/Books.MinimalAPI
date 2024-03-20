using Books.Application.Countries.GetCountry;
using Books.Application.Countries.GetCountryList;
using Books.Domain.Abstractions;
using Books.MinimalApi.Endpoints.Internal;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Books.MinimalApi.Endpoints.Countries;

public class GetCountryById : IEndpoint
{
    public static void AddServices(IServiceCollection services,
        IConfiguration configuration)
    {
        // Do nothing, all services were registered in infrastructure layer
    }

    public static void DefineEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{CountryConsts.BaseRoute}/{{countryId:guid}}", GetCountryByIdAsync)
         .RequireAuthorization()
         .WithName("GetCountry")
         .WithTags(CountryConsts.Tag)
         .WithOpenApi()
         .WithSummary("Get a country by providing an id")
         .WithDescription("Countries are identified by a URI containing a country " +
            "identifier. This identifier is a GUID. You can get one specific county via " +
            "this endpoint by providing the identifier");
            }

    internal static async Task<Results<Ok<CountryResponse>, NotFound<Error>>> GetCountryByIdAsync(
        ISender sender,
        Guid countryId,
        CancellationToken token)
    {
        var country = await sender.Send(new GetCountryQuery(countryId), token);
        return country.IsSuccess ? TypedResults.Ok(country.Value) : TypedResults.NotFound(country.Error);
    }
}
