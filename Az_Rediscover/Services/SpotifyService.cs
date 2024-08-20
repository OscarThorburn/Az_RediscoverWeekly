using Microsoft.Extensions.Configuration;
using Serilog;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Web;
using Az_Rediscover.Models.Spotify;
using Az_Rediscover.Models;

namespace Az_Rediscover.Services
{
    /// <summary>
    /// Service for interacting with the Spotify API.
    /// </summary>
    //TODO: Implement handling of retryable response codes (429, 502, 503)
    public class SpotifyService
    {
        private readonly string _refreshToken;
        private readonly string _clientSecret;
        private readonly string _discoverWeeklyPlaylistId;
        private readonly string _clientId;
        private readonly string _userName;
        private readonly string _authUrl;

        private readonly IHttpClientFactory _clientFactory;
        private readonly MemoryCacheService _memoryCacheService;

        public SpotifyService(IHttpClientFactory httpClientFactory, MemoryCacheService memoryCacheService)
        {
            _clientFactory = httpClientFactory;
            _memoryCacheService = memoryCacheService;
            _refreshToken = Environment.GetEnvironmentVariable("APPSETTING_SpotifyRefreshToken")!;
            _clientSecret = Environment.GetEnvironmentVariable("APPSETTING_SpotifyClientSecret")!;
            _discoverWeeklyPlaylistId = Environment.GetEnvironmentVariable("APPSETTING_DiscoverWeeklyPlaylistId")!;
            _clientId = Environment.GetEnvironmentVariable("APPSETTING_SpotifyClientId")!;
            _userName = Environment.GetEnvironmentVariable("APPSETTING_SpotifyUserName")!;
            _authUrl = Environment.GetEnvironmentVariable("APPSETTING_SpotifyAuthUrl")!;
        }

        public async Task<ResultModel<bool>> RediscoverAsync()
        {
            var rediscoverPlaylistId = await CreateNewPlaylistAsync();
            if (rediscoverPlaylistId.HasError)
                return new ResultModel<bool>
                {
                    ErrorMessage = rediscoverPlaylistId.ErrorMessage
                };
            Log.Information("Created new playlist with Id {RediscoverPlaylistIdValue}", rediscoverPlaylistId.Value);

            var currentTracks = await GetCurrentDiscoverWeeklyTrackIdsAsync();
            if (currentTracks.HasError)
                return new ResultModel<bool>
                {
                    ErrorMessage = currentTracks.ErrorMessage
                };
            Log.Information("Succesfully retrieved current tracks current discover weekly tracks");

            var result = await AddTracksToRediscoveredPlaylist(rediscoverPlaylistId.Value!, currentTracks.Value!);
            if (result.HasError)
                return new ResultModel<bool>
                {
                    ErrorMessage = result.ErrorMessage
                };

            Log.Information("Succesfully added tracks to rediscovered playlist");
            return new ResultModel<bool>
            {
                Value = true
            };
        }

        private async Task<ResultModel<bool>> AddTracksToRediscoveredPlaylist(string playlistId, List<string> trackIds)
        {
            var accessToken = await GetAccessTokenAsync();
            if (accessToken.HasError)
            {
                return new ResultModel<bool>
                {
                    ErrorMessage = accessToken.ErrorMessage
                };
            }

            var httpBody = new Dictionary<string, List<string>>
            {
                { "uris", trackIds.Select(x => $"spotify:track:{x}").ToList() }
            };

            var content = new StringContent(JsonSerializer.Serialize(httpBody), Encoding.UTF8, "application/json");

            using (var client = _clientFactory.CreateClient("SpotifyClient"))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Value);

                var response = await client.PostAsync($"playlists/{playlistId}/tracks", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = $"Failed to add tracks to rediscovered playlist with Id {playlistId}";
                    Log.Error(errorMessage);
                    return new ResultModel<bool>
                    {
                        ErrorMessage = errorMessage
                    };
                }

                return new ResultModel<bool>
                {
                    Value = true
                };
            }
        }

        private async Task<ResultModel<string>> CreateNewPlaylistAsync()
        {
            var accessToken = await GetAccessTokenAsync();
            if (accessToken.HasError)
            {
                return new ResultModel<string>
                {
                    ErrorMessage = accessToken.ErrorMessage
                };
            }

            var weekNumber = GetWeekNumber(DateTime.Now);

            var httpBody = new CreatePlaylistModel
            {
                Name = $"Rediscovered Week {weekNumber}",
                Description = $"Your rediscovered discover weekly playlist for week {weekNumber}",
                Public = false
            };

            var content = new StringContent(JsonSerializer.Serialize(httpBody), Encoding.UTF8, "application/json");

            using (var client = _clientFactory.CreateClient("SpotifyClient"))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Value);

                var response = await client.PostAsync($"users/{_userName}/playlists", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = "Failed to create new playlist";
                    Log.Error(errorMessage);
                    return new ResultModel<string>
                    {
                        ErrorMessage = errorMessage
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                return new ResultModel<string>
                {
                    Value = JsonNode.Parse(responseContent)!["id"]!.ToString()
                };
            }
        }

        private async Task<ResultModel<List<string>>> GetCurrentDiscoverWeeklyTrackIdsAsync()
        {
            var accessToken = await GetAccessTokenAsync();
            if (accessToken.HasError)
            {
                return new ResultModel<List<string>>
                {
                    ErrorMessage = accessToken.ErrorMessage
                };
            }

            using (var client = _clientFactory.CreateClient("SpotifyClient"))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Value);

                var uriBuilder = new UriBuilder(client.BaseAddress + $"playlists/{_discoverWeeklyPlaylistId}/tracks");

                var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query);
                queryParams["fields"] = "items(track(id))";

                uriBuilder.Query = queryParams.ToString();
                var finalUrl = uriBuilder.Uri.ToString();

                var response = await client.GetAsync(finalUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = "Failed to get current discover weekly.";
                    Log.Error(errorMessage);
                    return new ResultModel<List<string>>
                    {
                        ErrorMessage = errorMessage
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var currentDiscoverWeekly = JsonSerializer.Deserialize<PlaylistModel>(responseContent);

                return new ResultModel<List<string>>
                {
                    Value = currentDiscoverWeekly.Items.Select(x => x.Track.Id).ToList()
                };
            }
        }

        private async Task<ResultModel<string>> GetAccessTokenAsync()
        {
            if (_memoryCacheService.TryGetToken(out var token))
            {
                return new ResultModel<string>
                {
                    Value = token
                };
            }

            var httpBody = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", _refreshToken }
                };

            var content = new FormUrlEncodedContent(httpBody);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}")));

                var response = await client.PostAsync(_authUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = "Failed to get access token";
                    Log.Error(errorMessage);
                    return new ResultModel<string>
                    {
                        ErrorMessage = errorMessage
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var accessToken = JsonSerializer.Deserialize<AccessTokenResponseModel>(responseContent);
                _memoryCacheService.SetToken(accessToken.AccessToken, accessToken.ExpiresIn);
                return new ResultModel<string>
                {
                    Value = accessToken.AccessToken
                };
            }
        }

        private static int GetWeekNumber(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            var calendarWeekRule = culture.DateTimeFormat.CalendarWeekRule;
            var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
            return calendar.GetWeekOfYear(date, calendarWeekRule, firstDayOfWeek);
        }
    }
}