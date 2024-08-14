using System;
using System.Threading.Tasks;
using Az_RediscoverWeekly.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Az_RediscoverWeekly
{
    
    public class RediscoverWeeklyFunction
    {
        private readonly IServiceProvider _serviceProvider;
        public RediscoverWeeklyFunction(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [FunctionName("RediscoverWeeklyFunction")]
        public async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation("Started function at: {Time}", DateTime.Now);

                using var scope = _serviceProvider.CreateScope();
                var spotifyService = scope.ServiceProvider.GetRequiredService<SpotifyService>();

                var result = await spotifyService.RediscoverAsync();

                if (result.HasError)
                    log.LogError("Error during Rediscover: {ErrorMessage}", result.ErrorMessage);
                else
                    log.LogInformation("Rediscover was successful");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "An error occurred during the function execution");
            }
        }
    }
}
