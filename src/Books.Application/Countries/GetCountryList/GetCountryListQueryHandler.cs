using Books.Application.Abstractions.Messaging;
using Books.Domain.Abstractions;
using Books.Domain;

namespace Books.Application.Countries.GetCountryList;

// TODO: Use no tracking
// Don't load all data to db
internal sealed class GetCountryListQueryHandler
    : IQueryHandler<GetCountryListQuery, IReadOnlyList<CountryResponse>>
{
    private readonly ICountryRepository _repository;

    public GetCountryListQueryHandler(ICountryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<CountryResponse>>> Handle(GetCountryListQuery request, CancellationToken cancellationToken)
    {
        var countries = await _repository.ListAllAsync();
        if (countries == null)
        {
            return Result.Failure<IReadOnlyList<CountryResponse>>(CountryErrors.NotFound);
        }

        return countries.Select(country => new CountryResponse()
        {
            Id = country.Id,
            Name = country.Name.Value
        }).ToList();
    }
}