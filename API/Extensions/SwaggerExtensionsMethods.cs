using Microsoft.OpenApi.Models;

namespace API.Extensions;

public static class SwaggerExtensionsMethods
{
    public static void ConfigureSwagger(this IServiceCollection services) =>
        services.AddSwaggerGen(c => {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                Description = @"JWT Authorization Example : 'Bearer eyeleieieekeieieie",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference{
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "outh2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }

            });
        });

}