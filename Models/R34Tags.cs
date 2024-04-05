using Newtonsoft.Json;

namespace DiscordRandomPicBot.Models
{
    public partial class R34Tags
    {
        [JsonProperty("data")]
        public List<R34TagData> Data { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }
    }

    public partial class R34TagData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public partial class Links
    {
        [JsonProperty("self")]
        public Uri Self { get; set; }

        [JsonProperty("first")]
        public Uri First { get; set; }

        [JsonProperty("last")]
        public object Last { get; set; }

        [JsonProperty("prev")]
        public object Prev { get; set; }

        [JsonProperty("next")]
        public Uri Next { get; set; }
    }

    public partial class Meta
    {
        [JsonProperty("items_count")]
        public long ItemsCount { get; set; }

        [JsonProperty("total_items")]
        public object TotalItems { get; set; }

        [JsonProperty("current_page")]
        public long CurrentPage { get; set; }

        [JsonProperty("total_pages")]
        public object TotalPages { get; set; }

        [JsonProperty("items_per_page")]
        public long ItemsPerPage { get; set; }
    }
}

