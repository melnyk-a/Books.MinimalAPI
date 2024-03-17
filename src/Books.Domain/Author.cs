namespace Books.Domain;

public sealed class Author : Entity
{

    private readonly List<Book> _books = new();
    private Author(Guid id, FirstName firstName, LastName lastName, Guid countryId)
    : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        CountryId = countryId;
    }

    private Author() { }

    public FirstName FirstName { get; private set; }

    public LastName LastName { get; private set; }

    public Guid CountryId { get; private set; }

    public Country Country { get; private set; }

    public IReadOnlyCollection<Book> Books => _books;

    public static Author Create(FirstName firstName, LastName lastName, Guid countryId)
    {
        return new Author(Guid.NewGuid(), firstName, lastName, countryId);
    }
}
