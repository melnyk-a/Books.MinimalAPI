using Books.Application.Abstractions.Messaging;
using Books.Application.Countries.GetCountryList;
using Books.Domain.Abstractions.Persistence;
using Books.Domain;
using Books.Domain.Abstractions;

namespace Books.Application.Countries.GetCountry;

internal sealed class GetCountryQueryHandler
    : IQueryHandler<GetCountryQuery, CountryResponse>
{
    private readonly ICountryRepository _repository;

    public GetCountryQueryHandler(ICountryRepository repository)
    {
        _repository = repository;
    }


    public async Task<Result<CountryResponse>> Handle(GetCountryQuery request, CancellationToken cancellationToken)
    {
        var country = await _repository.GetByIdAsync(request.Id);
        if (country == null)
        {
            return Result.Failure<CountryResponse>(CountryErrors.NotFound);
        }

        return Result.Success(new CountryResponse()
        {
            Id = country.Id,
            Name = country.Name.Value
        });
    }
}