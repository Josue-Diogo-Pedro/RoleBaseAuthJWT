
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Identity;

namespace API.Extensions;

public static class IdentityExtensionMethods
{
    public static void ConfigureIdentity(this IServiceCollection service) =>
        service.AddIdentity<AppUser, IdentityRole>()
               .AddEntityFrameworkStores<AppDbContext>()
               .AddDefaultTokenProviders();
}