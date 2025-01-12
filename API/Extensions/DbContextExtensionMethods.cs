using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class DbContextExtensionMethods
{
    public static void ConfigureDbContext(this IServiceCollection services) =>
        services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=auth.db"));
}