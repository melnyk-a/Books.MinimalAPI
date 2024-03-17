namespace Books.Domain;

public sealed class Book : Entity
{
    private Book(Guid id, Title title, Guid authorId) : base(id)
    {
        Title = title;
        AuthorId = authorId;
    }

    private Book() { }

    public Title Title { get; private set; }

    public Guid AuthorId { get; private set; }

    public Author Author { get; private set; }

    public static Book Create(Title title, Guid authorId) => new Book(Guid.NewGuid(), title, authorId);
}
