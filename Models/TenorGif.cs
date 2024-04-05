using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRandomPicBot.Models
{
    public partial class TenorGifResult
    {
        [JsonProperty("results")]
        public TenorGif[] Gifs { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }
    }

    public partial class TenorGif
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("media_formats")]
        public Dictionary<string, MediaFormat> MediaFormats { get; set; }

        [JsonProperty("created")]
        public double Created { get; set; }

        [JsonProperty("content_description")]
        public string ContentDescription { get; set; }

        [JsonProperty("itemurl")]
        public Uri Itemurl { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("flags")]
        public object[] Flags { get; set; }

        [JsonProperty("hasaudio")]
        public bool Hasaudio { get; set; }
    }

    public partial class MediaFormat
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("preview")]
        public string Preview { get; set; }

        [JsonProperty("dims")]
        public long[] Dims { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }
    }
}
