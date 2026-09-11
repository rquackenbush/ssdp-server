using SsdpServer.Services;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHostedService<SsdpService>();

        var app = builder.Build();

        var logger = app.Services.GetRequiredService<ILogger<object>>();

        app.MapWhen(c => true, async builder =>
        {
            builder.Run(async context =>
            {
                // Handle the matched request
                await context.Response.WriteAsync("Matched");

                //logger.LogInformation("Request received: {Method}: Path: {Path}", context.Request.Method, context.Request.Path);
            });
        });

        //app.Use(async (context, next) =>
        //{
        //    logger.LogInformation("Request received: {Method}: Path: {Path}", context.Request.Method, context.Request.Path);
           
        //    await next();
        //});

        await app.RunAsync("http://*:8060");

        return 0;
    }
}



