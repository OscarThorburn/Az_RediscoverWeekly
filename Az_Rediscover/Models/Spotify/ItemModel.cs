using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Az_Rediscover.Models.Spotify
{
    public record struct ItemModel
    {
        [JsonPropertyName("track")]
        public TrackModel Track { get; set; }
    }
}
