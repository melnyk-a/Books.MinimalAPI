using Books.Domain.Abstractions.Persistence;

namespace Books.Domain;

public interface ICountryRepository : IRepository<Country>
{
    Task<Country?> GetByNameAsync(string countryName, CancellationToken cancellationToken);

    Task<IReadOnlyList<Country>> ListByNameAsync(string countryName, CancellationToken cancellationToken);
}
