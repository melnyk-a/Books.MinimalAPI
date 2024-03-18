using Books.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Books.Infrastructure.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("authors");

        builder.HasKey(author => author.Id);

        builder.Property(author => author.FirstName)
            .HasConversion(name => name.Value, value => new FirstName(value));

        builder.Property(author => author.LastName)
            .HasConversion(name => name.Value, value => new LastName(value));

        builder.HasOne(author => author.Country)
            .WithMany(country => country.Authors)
            .HasForeignKey(author => author.CountryId)
            .IsRequired();

        var navigation = builder.Metadata.FindNavigation(nameof(Author.Books));
        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
