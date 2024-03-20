using Books.Application.Countries.GetCountryList;
using Books.MinimalApi.Endpoints.Internal;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Books.MinimalApi.Endpoints.Countries;

public class GetCountryByName : IEndpoint
{
    public static void AddServices(IServiceCollection services,
        IConfiguration configuration)
    {
        // Do nothing, all services were registered in infrastructure layer
    }

    public static void DefineEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{CountryConsts.BaseRoute}/{{countryName}}", GetCountryByNameAsync)
            .RequireAuthorization()
            .WithTags(CountryConsts.Tag)
            .WithOpenApi(options =>
            {
                options.Deprecated = true;
                return options;
            });
    }

    private static async Task<Ok<IReadOnlyList<CountryResponse>>> GetCountryByNameAsync(
        ISender sender,
        string countryName,
        CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
