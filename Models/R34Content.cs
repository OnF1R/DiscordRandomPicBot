using Newtonsoft.Json;

namespace DiscordRandomPicBot.Models
{
    public partial class R34Content
    {
        [JsonProperty("data")]
        public R34ContentData[] Data { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }
    }

    public partial class R34ContentData
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("score")]
        public long? Score { get; set; }

        [JsonProperty("high_res_file")]
        public File HighResFile { get; set; }

        [JsonProperty("low_res_file")]
        public File LowResFile { get; set; }

        [JsonProperty("preview_file")]
        public File PreviewFile { get; set; }

        [JsonProperty("tags")]
        public Tags Tags { get; set; }

        [JsonProperty("sources")]
        public Uri[] Sources { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("media_type")]
        public string MediaType { get; set; }
    }

    public partial class File
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("width")]
        public long? Width { get; set; }

        [JsonProperty("height")]
        public long? Height { get; set; }
    }

    public partial class Tags
    {
        [JsonProperty("artist")]
        public string[] Artist { get; set; }

        [JsonProperty("character")]
        public string[] Character { get; set; }

        [JsonProperty("copyright")]
        public string[] Copyright { get; set; }

        [JsonProperty("general")]
        public string[] General { get; set; }

        [JsonProperty("meta")]
        public string[] Meta { get; set; }
    }

    //public enum MediaType { Image, Video };

    //public enum Rating { Explicit, Questionable };
}
