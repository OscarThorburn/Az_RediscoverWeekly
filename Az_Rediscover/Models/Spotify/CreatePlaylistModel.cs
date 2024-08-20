using System.Text.Json.Serialization;

namespace Az_Rediscover.Models.Spotify
{
	/// <summary>
	/// Model for creating a new playlist in Spotify.
	/// </summary>
	public record struct CreatePlaylistModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("public")]
        public bool Public { get; set; }
    }
}
