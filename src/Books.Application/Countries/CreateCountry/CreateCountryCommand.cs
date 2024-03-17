using Books.Application.Abstractions.Messaging;

namespace Books.Application.Countries.CreateCountry;

public sealed record CreateCountryCommand(string CountryName) : ICommand<Guid>;
