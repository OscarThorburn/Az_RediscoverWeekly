
using System.Text.Json.Serialization;

namespace Az_RediscoverWeekly.Models.Spotify
{
    public record struct TrackModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
