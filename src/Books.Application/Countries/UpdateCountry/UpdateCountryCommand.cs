using Books.Application.Abstractions.Messaging;

namespace Books.Application.Countries.UpdateCountry;

public sealed record UpdateCountryCommand(Guid CountryId, string CountryName)
    : ICommand<Guid>;
