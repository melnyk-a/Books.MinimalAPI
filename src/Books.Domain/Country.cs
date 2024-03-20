namespace Books.Domain;

public sealed class Country : Entity
{
    private readonly List<Author> _authors = new();
    private Country(Guid id, Name name) : base(id)
    {
        Name = name;
    }

    private Country() { }
    public Name Name { get; private set; }

    public IReadOnlyCollection<Author> Authors => _authors.ToList();

    public void UpdateName(Name name) => Name = name;
    public static Country Create(Name name) => new(Guid.NewGuid(), name);
}
