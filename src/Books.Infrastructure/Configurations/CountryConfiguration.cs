using Books.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Books.Infrastructure.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("countries");

        builder.HasKey(country => country.Id);

        builder.Property(country => country.Name)
            .HasConversion(country => country.Value, value => new Name(value));

        builder.HasIndex(user => user.Name).IsUnique();

        var navigation = builder.Metadata.FindNavigation(nameof(Country.Authors));
        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
