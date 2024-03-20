using Books.Application.Abstractions.Messaging;

namespace Books.Application.Countries.GetCountryList;

public sealed record GetCountryListQuery(string? CountryName) : IQuery<IReadOnlyList<CountryResponse>>;
