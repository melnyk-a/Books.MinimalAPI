using Books.Application.Abstractions.Messaging;

namespace Books.Application.Countries.GetCountryWithAuthorList;

public sealed record GetCountryWithAuthorListQuery()
    : IQuery<IReadOnlyList<CountryWithAuthorResponse>>;
