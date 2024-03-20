using Books.Domain;
using Microsoft.EntityFrameworkCore;

namespace Books.Infrastructure.Repositories;

internal sealed class CountryRepository : Repository<Country>, ICountryRepository
{
    public CountryRepository(ApplicationDbContext dbContext)
       : base(dbContext)
    {
    }

    // TODO: check expression tree
    public async Task<Country?> GetByNameAsync(string countryName, CancellationToken cancellationToken)
    {
        var countryNameToCompare = new Name(countryName);
        return await DbContext.Set<Country>()
            .FirstOrDefaultAsync(x => x.Name == countryNameToCompare);
    }

    //TODO: Remove after implementing no tracking
    public async override Task<IReadOnlyList<Country>> ListAllAsync(CancellationToken cancellationToken)
    {
        return await DbContext.Set<Country>()
            .Include(x => x.Authors)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Country>> ListByNameAsync(string countryName, CancellationToken cancellationToken)
    {
        var countryNameToCompare = new Name(countryName);
        return await DbContext.Set<Country>()
            .Include(x => x.Authors)
            .Where(x => x.Name == countryNameToCompare)
            .ToListAsync(cancellationToken);
    }
}
