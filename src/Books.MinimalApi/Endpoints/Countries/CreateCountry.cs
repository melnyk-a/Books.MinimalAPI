using Books.Application.Countries.CreateCountry;
using Books.Domain.Abstractions;
using Books.MinimalApi.Endpoints.Internal;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Books.MinimalApi.Endpoints.Countries;

public class CreateCountry : IEndpoint
{
    public static void AddServices(IServiceCollection services,
    IConfiguration configuration)
    {
        // Do nothing, all services were registered in infrastructure layer
    }

    public static void DefineEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapPost(CountryConsts.BaseRoute, CreateCountryAsync)
            .RequireAuthorization()
            .Accepts<CreateCountryRequest>(CountryConsts.JsonContentType)
            .WithTags(CountryConsts.Tag);
    }

    private static async Task<Results<CreatedAtRoute<Guid>, BadRequest<Error>>>
        CreateCountryAsync(
        ISender sender,
        IValidator<CreateCountryRequest> validator,
        CreateCountryRequest request,
        CancellationToken token)
    {
        await validator.ValidateAndThrowAsync(request);

        var command = new CreateCountryCommand(request.Name);

        var result = await sender.Send(command, token);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.CreatedAtRoute(
            result.Value,
            "GetCountry",
            new { CountryId = result.Value });
    }
}
