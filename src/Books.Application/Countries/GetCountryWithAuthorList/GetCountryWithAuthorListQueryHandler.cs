using Books.Application.Abstractions.Messaging;
using Books.Domain.Abstractions;
using Books.Domain;

namespace Books.Application.Countries.GetCountryWithAuthorList;

// TODO: Use no tracking
// Don't load all data to db
internal sealed class GetCountryWithAuthorListQueryHandler
    : IQueryHandler<GetCountryWithAuthorListQuery, IReadOnlyList<CountryWithAuthorResponse>>
{
    private readonly ICountryRepository _repository;

    public GetCountryWithAuthorListQueryHandler(ICountryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<CountryWithAuthorResponse>>> Handle(GetCountryWithAuthorListQuery request, CancellationToken cancellationToken)
    {
        var countries = await _repository.ListAllAsync();
        if (countries == null)
        {
            return Result.Failure<IReadOnlyList<CountryWithAuthorResponse>>(CountryErrors.NotFound);
        }

        return countries.Select(country => new CountryWithAuthorResponse()
        {
            Id = country.Id,
            Name = country.Name.Value,
            AuthorsResponse = country.Authors.Select(author => new AuthorResponse()
            {
                Id = author.Id,
                FirstName = author.FirstName.Value,
                LastName = author.LastName.Value,
            }).ToList()
        }).ToList();
    }
}