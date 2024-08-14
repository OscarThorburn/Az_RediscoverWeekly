using Az_RediscoverWeekly.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureLogging((hostingContext, logging) =>
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(LogEventLevel.Information, outputTemplate:
             "[{Timestamp:yyyy-MM-dd HH:mm:ss} {SourceContext} [{Level}] {Message}{NewLine}{Exception}")
            .CreateLogger();

        logging.AddSerilog(Log.Logger, true);
    })
    .ConfigureAppConfiguration((hostContext, config) =>
       {
           if (hostContext.HostingEnvironment.IsDevelopment())
           {
               config.AddJsonFile("local.settings.json");
               config.AddEnvironmentVariables();
           }
       })
    .ConfigureServices((context, services) =>
    {
        services.AddDataProtection();
        services.AddMemoryCache();
        services.AddScoped<DataProtectorService>();
        services.AddScoped<SpotifyService>();
        services.AddSingleton<SecretService>();
        services.AddScoped<DataProtectorService>();
        services.AddScoped<MemoryCacheService>();
        services.AddHttpClient("SpotifyClient", client =>
        {
            client.BaseAddress = new Uri(context.Configuration.GetValue<string>("Spotify:BaseAdressApi")!);
        });
    })
    .Build();

await host.RunAsync();