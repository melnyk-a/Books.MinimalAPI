using Books.Application.Countries.GetCountryWithAuthorList;
using Books.MinimalApi.Endpoints.Internal;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Books.MinimalApi.Endpoints.Countries;

public class GetCountryListWithAuthors : IEndpoint
{
    public static void AddServices(IServiceCollection services,
        IConfiguration configuration)
    {
        // Do nothing, all services were registered in infrastructure layer
    }

    public static void DefineEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{CountryConsts.BaseRoute}/authors", GetCountryWithAthorsAsync)
            .RequireAuthorization()
            .WithTags(CountryConsts.Tag);
    }

    private static async Task<Ok<IReadOnlyList<CountryWithAuthorResponse>>> GetCountryWithAthorsAsync(
    ISender sender,
    CancellationToken token)
    {
        var countries = await sender.Send(new GetCountryWithAuthorListQuery(), token);

        return TypedResults.Ok(countries.Value);
    }
}
