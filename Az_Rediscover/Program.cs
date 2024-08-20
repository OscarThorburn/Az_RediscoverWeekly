using Az_Rediscover.Services;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
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
           config.AddEnvironmentVariables();
           if (hostContext.HostingEnvironment.IsDevelopment())
           {
               config.AddJsonFile("local.settings.json");
           }
           else 
           {
               config.AddAzureAppConfiguration(options =>
               {
                   options.Connect(new Uri("https://RediscoverAppConfig.azconfig.io"), new DefaultAzureCredential())
                        .ConfigureKeyVault(kv =>
                        {
                            kv.SetCredential(new DefaultAzureCredential());
                        });
               });
           }
           
       })
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();
        services.AddDataProtection();
        services.AddMemoryCache();
        services.AddScoped<DataProtectorService>();
        services.AddScoped<SpotifyService>();
        services.AddScoped<MemoryCacheService>();
        services.AddHttpClient("SpotifyClient", client =>
        {
            client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SpotifyBaseApiUrl")!);
        });
    })
    .Build();

host.Run();