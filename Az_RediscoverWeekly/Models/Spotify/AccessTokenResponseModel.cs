using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Az_RediscoverWeekly.Models.Spotify
{
    /// <summary>
    /// Represents the response model for the access token request used to authenticate with the Spotify API.
    /// </summary>
    public record struct AccessTokenResponseModel
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName("scope")]
        public string Scope { get; set; }
    }
}
