namespace API.Extensions;

public static class WebAppExtensionMethods
{
    public static WebApplication ConfigureEnvironment(this WebApplication app){
        if(app.Environment.IsDevelopment()){
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        return app;
    }

    public static WebApplication ConfigureCors(this WebApplication app){
        
        app.UseCors(options => {
            options.AllowAnyHeader();
            options.AllowAnyMethod();
            options.AllowAnyOrigin();
        });
        
        return app;
    }

    public static WebApplication ConfigureAuthAndRun(this WebApplication app){

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();

        return app;
    }
}