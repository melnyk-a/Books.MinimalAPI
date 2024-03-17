using Books.Domain.Abstractions.Persistence;

namespace Books.Domain;

public interface IBookRepository : IRepository<Book>
{
}
