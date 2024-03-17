using Books.MinimalApi.EndpointFilters;
using Books.MinimalApi.EndpointHandlers.Countries;

namespace Books.MinimalApi.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterCountryEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var countriesEndpoints = endpointRouteBuilder
            .MapGroup("/countries")
            .RequireAuthorization();

        var countriesWithGuidIdEndpoints = countriesEndpoints
            .MapGroup("/{countryId:guid}")
            .RequireAuthorization("RequireAdminFromUkraine");

        var countriesAuthorsEndpoints = countriesEndpoints.MapGroup("/authors");

        countriesEndpoints.MapGet("", CountryHandlers.GetCountriesAsync);

        countriesWithGuidIdEndpoints
            .MapGet("", CountryHandlers.GetCountryByIdAsync)
            .WithName("GetCountry")
            .WithOpenApi()
            .WithSummary("Get a country by providing an id")
            .WithDescription("Countries are identified by a URI containing a country " +
                "identifier. This identifier is a GUID. You can get one specific county via " +
                "this endpoint by providing the identifier");

        countriesEndpoints
            .MapGet("/{countryName}", CountryHandlers.GetCountryByName)
            .WithOpenApi(options =>
            {
                options.Deprecated = true;
                return options;
            });

        countriesAuthorsEndpoints.MapGet("", CountryHandlers.GetCountryWithAthorsAsync);

        countriesEndpoints
            .MapPost("", CountryHandlers.CreateCountryAsync)
            .Accepts<CreateCountryRequest>("application/json");

        var countriesWithGuidIdAndLockFilters = countriesWithGuidIdEndpoints
            .AddEndpointFilter(
                new CountryLockedFilter(Guid.Parse("09DABAD8-33F5-4699-AFF8-C1AF0C1D08D7")))
            .AddEndpointFilter(
                new CountryLockedFilter(Guid.Parse("4CE0FCE9-46A6-4A12-B57F-BE325A69ADF1")));

        countriesWithGuidIdAndLockFilters
            .MapPut("", CountryHandlers.UpdateCountryAsync);
        countriesWithGuidIdAndLockFilters
            .MapDelete("", CountryHandlers.DeleteCountryAsync)
            .AddEndpointFilter<LogNotFoundResponseFilter>();
    }

    public static void RegisterBooksEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var countriesEndpoints = endpointRouteBuilder
            .MapGroup("/books")
            .AllowAnonymous();

        var countriesWithGuidIdEndpoints = countriesEndpoints.MapGroup("/{countryId:guid}");

        countriesEndpoints.MapGet("", () =>
        {
            throw new NotImplementedException();
        });
    }
}
