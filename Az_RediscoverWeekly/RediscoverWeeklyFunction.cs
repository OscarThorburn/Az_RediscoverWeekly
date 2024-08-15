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
        private readonly SpotifyService _spotifyService;
        //private readonly IServiceProvider _serviceProvider;
        public RediscoverWeeklyFunction(SpotifyService spotifyService)
        {
            //_serviceProvider = serviceProvider;
            _spotifyService = spotifyService;
        }

        [FunctionName("RediscoverWeeklyFunction")]
        public async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation("Started function at: {Time}", DateTime.Now);


                var result = await _spotifyService.RediscoverAsync();

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
