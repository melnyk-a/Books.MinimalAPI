using Books.Domain;

namespace Books.Infrastructure;

public static class ApplicationDbContextExtensions
{
    public static void Seed(this ApplicationDbContext context)
    {
        var countries = new List<Country>
    {
        Country.Create(new Name("Afghanistan")),
        Country.Create(new Name("Albania"))
    };
        var authors = new List<Author>
    {
        Author.Create(new FirstName("Atiq"), new LastName("Rahimi"), countries[0].Id),
        Author.Create(new FirstName("Khaled"), new LastName("Hosseini"), countries[0].Id),
        Author.Create(new FirstName("Anna"), new LastName("Badkhen"), countries[0].Id),
        Author.Create(new FirstName("Emmanuel"), new LastName("Guibert"), countries[0].Id),
        Author.Create(new FirstName("Batya"), new LastName("Swift"), countries[0].Id),
        Author.Create(new FirstName("Ismail"), new LastName("Kadare"), countries[1].Id),
        Author.Create(new FirstName("Fatos"), new LastName("Kongoli"), countries[1].Id),
        Author.Create(new FirstName("Elvira"), new LastName("Dones"), countries[1].Id)
    };

        var books = new List<Book>
    {
        Book.Create(new Title("A Thousand Rooms of Dream and Fear"), authors[0].Id),
        Book.Create(new Title("The Patience Stone"), authors[0].Id),
        Book.Create(new Title("The Kite Runner"), authors[1].Id),
        Book.Create(new Title("A Thousand Splendid Suns"), authors[1].Id),
        Book.Create(new Title("Waiting for the Taliban"), authors[2].Id),
        Book.Create(new Title("The Photographer"), authors[3].Id),
        Book.Create(new Title("Yasgur Behind the Burqa"), authors[4].Id),
        Book.Create(new Title("The Palace of Dreams"), authors[5].Id),
        Book.Create(new Title("Broken April"), authors[6].Id),
        Book.Create(new Title("The Loser"), authors[6].Id),
        Book.Create(new Title("Sworn Virgin"), authors[7].Id)
    };
        context.AddRange(countries);
        context.AddRange(authors);
        context.AddRange(books);
        context.SaveChanges();
    }
}
