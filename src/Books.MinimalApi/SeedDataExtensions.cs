using Books.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Books.MinimalApi;

public static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Database.EnsureDeleted();
        context.Database.Migrate();

        //context.Seed();
    }
}
