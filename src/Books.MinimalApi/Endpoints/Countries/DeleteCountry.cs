using Books.Application.Countries.DeleteCountry;
using Books.Domain.Abstractions;
using Books.MinimalApi.EndpointFilters;
using Books.MinimalApi.Endpoints.Internal;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Books.MinimalApi.Endpoints.Countries;

public class DeleteCountry : IEndpoint
{
    public static void AddServices(IServiceCollection services,
        IConfiguration configuration)
    {
        // Do nothing, all services were registered in infrastructure layer
    }

    public static void DefineEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{CountryConsts.BaseRoute}/{{countryId:guid}}", DeleteCountryAsync)
            .RequireAuthorization("RequireAdminFromUkraine")
            .AddEndpointFilter(
                new CountryLockedFilter(Guid.Parse("09DABAD8-33F5-4699-AFF8-C1AF0C1D08D7")))
            .AddEndpointFilter(
                new CountryLockedFilter(Guid.Parse("4CE0FCE9-46A6-4A12-B57F-BE325A69ADF1")))
            .AddEndpointFilter<LogNotFoundResponseFilter>()
            .WithTags(CountryConsts.Tag); ;
    }

    private static async Task<Results<BadRequest<Error>, NotFound<Error>, NoContent>>
        DeleteCountryAsync(
        ISender sender,
        Guid countryId,
        CancellationToken token)
    {
        var command = new DeleteCountryCommand(countryId);

        var result = await sender.Send(command, token);

        if (result.IsFailure)
        {
            return result.Error switch
            {
                NotFoundError => TypedResults.NotFound(result.Error),
                _ => TypedResults.BadRequest(result.Error)
            };
        }

        return TypedResults.NoContent();
    }
}
