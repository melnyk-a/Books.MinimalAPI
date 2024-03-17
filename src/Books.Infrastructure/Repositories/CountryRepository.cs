using Books.Domain;
using Microsoft.EntityFrameworkCore;

namespace Books.Infrastructure.Repositories;

internal sealed class CountryRepository : Repository<Country>, ICountryRepository
{
    public CountryRepository(ApplicationDbContext dbContext)
       : base(dbContext)
    {
    }

    //TODO: Remove after implementing no tracking
    public async override Task<IReadOnlyList<Country>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Country>()
            .Include(x => x.Authors)
            .ToListAsync(cancellationToken);
    }
}
