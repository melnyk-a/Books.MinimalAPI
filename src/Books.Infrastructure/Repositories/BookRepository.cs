using Books.Domain;

namespace Books.Infrastructure.Repositories;

internal sealed class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext dbContext)
       : base(dbContext)
    {
    }
}
