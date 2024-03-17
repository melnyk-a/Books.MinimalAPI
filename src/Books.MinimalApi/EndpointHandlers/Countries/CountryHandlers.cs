using Books.Application.Countries.CreateCountry;
using Books.Application.Countries.DeleteCountry;
using Books.Application.Countries.GetCountry;
using Books.Application.Countries.GetCountryList;
using Books.Application.Countries.GetCountryWithAuthorList;
using Books.Application.Countries.UpdateCountry;
using Books.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Books.MinimalApi.EndpointHandlers.Countries;

public static class CountryHandlers
{
    internal static async Task<Results<BadRequest<Error>, NotFound<Error>, NoContent>> DeleteCountryAsync(
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

    internal static async Task<Results<BadRequest<Error>, NotFound<Error>, NoContent>> UpdateCountryAsync(
        ISender sender,
        [FromRoute]Guid countryId,
        [FromBody]UpdateCountryRequest request,
        CancellationToken token)
    {
        var command = new UpdateCountryCommand(
            countryId,
            request.Name);

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
    internal static async Task<Results<CreatedAtRoute<Guid>, BadRequest<Error>>> CreateCountryAsync(
        ISender sender,
        CreateCountryRequest request,
        CancellationToken token)
    {
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

    internal static async Task<Ok<IReadOnlyList<CountryWithAuthorResponse>>> GetCountryWithAthorsAsync(
        ISender sender,
        CancellationToken token)
    {
        var countries = await sender.Send(new GetCountryWithAuthorListQuery(), token);

        return TypedResults.Ok(countries.Value);
    }

    internal static async Task<Ok<IReadOnlyList<CountryResponse>>> GetCountryByName(
        ISender sender,
        string countryName,
        CancellationToken token)
    {
        throw new NotImplementedException();
        //return await sender.Send(new GetCountryListQuery(), token);
    }

    internal static async Task<Results<Ok<CountryResponse>, NotFound>> GetCountryByIdAsync(
        ISender sender,
        Guid countryId,
        CancellationToken token)
    {
        var country = await sender.Send(new GetCountryQuery(countryId), token);

        return country.IsSuccess ? TypedResults.Ok(country.Value) : TypedResults.NotFound();
    }

    internal static async Task<Ok<IReadOnlyList<CountryResponse>>> GetCountriesAsync(
        ISender sender,
        [FromQuery] string? name,
        CancellationToken token)
    {
        var countries = await sender.Send(new GetCountryListQuery(name), token);

        return TypedResults.Ok(countries.Value);
    }
}
