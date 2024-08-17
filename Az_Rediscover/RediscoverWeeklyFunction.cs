using System;
using System.Threading.Tasks;
using Az_Rediscover.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Az_RediscoverWeekly
{
    
    public class RediscoverWeeklyFunction
    {
        private readonly SpotifyService _spotifyService;
        private readonly ILogger<RediscoverWeeklyFunction> _logger;
        public RediscoverWeeklyFunction(SpotifyService spotifyService, ILogger<RediscoverWeeklyFunction> logger)
        {
            _spotifyService = spotifyService;
            _logger = logger;
        }

        [Function("RediscoverWeeklyFunction")]
        public async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation("Started function at: {Time}", DateTime.Now);


                var result = await _spotifyService.RediscoverAsync();

                if (result.HasError)
                    _logger.LogError("Error during Rediscover: {ErrorMessage}", result.ErrorMessage);
                else
                    _logger.LogInformation("Rediscover was successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the function execution");
            }
        }
    }
}
