using System.Text.Json.Serialization;

namespace Az_Rediscover.Models.Spotify
{
    /// <summary>
    /// A single track in spotify. Contains a lot more possible variables but only need the ID
    /// </summary>
    public record struct TrackModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
