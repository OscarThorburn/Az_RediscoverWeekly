using System.Text.Json.Serialization;

namespace Az_Rediscover.Models.Spotify
{
	/// <summary>
	/// One of the models returned by the Spotify API. Contains a list of items 
	/// </summary>
	public record struct PlaylistModel
    {
        [JsonPropertyName("items")]
        public List<ItemModel> Items { get; set; }
    }
}
