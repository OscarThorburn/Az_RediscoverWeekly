using System.Text.Json.Serialization;

namespace Az_Rediscover.Models.Spotify
{
	/// <summary>
	/// Represents a single item in a Spotify response. In this case it will always be representing tracks
	/// </summary>
	public record struct ItemModel
    {
        [JsonPropertyName("track")]
        public TrackModel Track { get; set; }
    }
}
