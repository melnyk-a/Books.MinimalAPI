using Books.Application.Abstractions.Messaging;
using Books.Application.Countries.GetCountryList;

namespace Books.Application.Countries.GetCountry;

public sealed record GetCountryQuery(Guid Id) : IQuery<CountryResponse>;
