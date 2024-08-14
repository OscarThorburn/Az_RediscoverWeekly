using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Az_RediscoverWeekly.Models.Spotify
{
    public record struct PlaylistModel
    {
        [JsonPropertyName("items")]
        public List<ItemModel> Items { get; set; }
    }
}
