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
        IReadOnlyList<Country> countries;
        var name = request.CountryName;
        if (!string.IsNullOrEmpty(name))
        {
            countries = await _repository.ListByNameAsync(request.CountryName!, cancellationToken);
        }
        else
        {
            countries = await _repository.ListAllAsync(cancellationToken);
        }

        return countries.Select(country => new CountryResponse()
        {
            Id = country.Id,
            Name = country.Name.Value
        }).ToList();
    }
}