using Books.Application.Abstractions.Messaging;

namespace Books.Application.Countries.GetCountryList;

//TODO: IMPLEMENT
public sealed record GetCountryListQuery(string? CountryName) : IQuery<IReadOnlyList<CountryResponse>>;
