using AnimePicturesScroller.Models;
using DiscordRandomPicBot.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace DiscordRandomPicBot
{
    public class FromSafebooru
    {
        const string TAGS_URL = "https://api.r34.app/booru/gelbooru.com/tags?baseEndpoint=gelbooru.com&order=count&limit=20";
        const string BASE_URL = "https://api.r34.app/booru/gelbooru/posts?baseEndpoint=safebooru.org&limit=20";

        private static Dictionary<string, string> Tags = new()
        {
        };

        public static bool AddTag(string tag)
        {
            if (Tags.ContainsKey(tag))
                return false;

            if (Tags.Count == 0)
            {
                Tags.Add(tag, tag);
            }
            else
            {
                Tags.Add(tag, $"|{tag}");
            }

            return true;
        }

        public static bool RemoveTag(string tag)
        {
            if (Tags.Remove(tag))
                return true;

            return false;
        }

        public static string CurrentTags()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var tag in Tags)
                sb.Append($"\n {tag.Key}");

            return sb.ToString();
        }

        private static void ValidateFirstTag()
        {
            if (Tags.First().Value.StartsWith("|"))
                Tags[Tags.First().Key] = Tags.First().Value.Remove(0, 1);
        }

        public static async Task<(bool, string)> ConfirmTag(string tag)
        {
            char tempPrefix = '@';

            if (tag.StartsWith('-'))
            {
                tempPrefix = '-';
                tag = tag.Remove(0, 1);
            }

            var Request = await SearchTagsResult(tag);

            if (Request == null)
                return (false, "Not found or mistake in query, if you search 2 or more words, split them with symbol '_', for example: genshin_impact");

            if (Request.Data == null)
                return (false, $"Not found matches");

            foreach (var tagData in Request.Data)
            {
                if (tagData.Name == tag)
                {
                    if (tempPrefix == '-')
                        tag = tempPrefix + tag;

                    AddTag(tag);
                    return (true, $"Tag {tag} added to list");
                }
            }

            Dictionary<string, long> TagsRequestData = new();
            StringBuilder sb = new StringBuilder();

            foreach (var tagData in Request.Data)
            {
                TagsRequestData.Add(tagData.Name, tagData.Count);
                sb.Append($"\n {tagData.Name} ({tagData.Count})");
            }

            return (false, $"Maybe you meant: {sb}");
        }

        public static async Task<(bool, string)> TagsSearch(string tag)
        {
            var Request = await SearchTagsResult(tag);

            if (Request == null)
                return (false, "Not found or mistake in query, if you search 2 or more words, split them with symbol '_', for example: genshin_impact");

            if (Request.Data == null)
                return (false, $"Not found matches");

            Dictionary<string, long> TagsRequestData = new();
            StringBuilder sb = new StringBuilder();

            foreach (var tagData in Request.Data)
            {
                TagsRequestData.Add(tagData.Name, tagData.Count);
                sb.Append($"\n {tagData.Name} ({tagData.Count})");
            }

            return (false, $"Existable similar tags: {sb}");
        }

        private static async Task<R34Tags> SearchTagsResult(string tag)
        {
            string animeImagesData = "";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(TAGS_URL + $"&tag={tag}");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                animeImagesData = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<R34Tags>(animeImagesData);
        }

        private static async Task<R34Content> SearchContent(int pageId)
        {
            //string proxyURL = "http://159.65.221.25:80";

            //WebProxy webProxy = new WebProxy(proxyURL);

            //HttpClientHandler httpClientHandler = new HttpClientHandler
            //{
            //    Proxy = webProxy
            //};
            string animeImagesData = "";
            //HttpClient client = new HttpClient(httpClientHandler);
            HttpClient client = new HttpClient();
            string Url = "";
            if (Tags.Count > 0)
            {
                Url = BASE_URL + $"&pageID={pageId}" + $"&tags={GenerateTagsRequest()}";
            }
            else
            {
                Url = BASE_URL + $"&pageID={pageId}";
            }
            Console.WriteLine();
            HttpResponseMessage response = await client.GetAsync(Url);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                animeImagesData = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<R34Content>(animeImagesData);
        }

        private static string GenerateTagsRequest()
        {
            StringBuilder sb = new StringBuilder();
            ValidateFirstTag();
            foreach (var tag in Tags)
                sb.Append(tag.Value);

            return sb.ToString();
        }

        public static async Task<(Uri, string)> RandomContent()
        {
            var rand = new Random();
            int count = 0;
            R34Content result;
            do
            {
                count++;

                switch (count)
                {
                    case <= 1:
                        result = await SearchContent(rand.Next(0, 999));
                        break;
                    case <= 2:
                        result = await SearchContent(rand.Next(0, 150));
                        break;
                    case <= 3:
                        result = await SearchContent(rand.Next(0, 15));
                        break;
                    default:
                        result = await SearchContent(rand.Next(0, 1));
                        break;
                }

            } while (result == null);

            StringBuilder sb = new StringBuilder();

            int dataLength = result.Data.Length;

            int index = rand.Next(0, dataLength);

            var content = result.Data[index];

            foreach (var artist in content.Tags.Artist)
                sb.Append(artist + ", ");

            foreach (var character in content.Tags.Character)
                sb.Append(character + ", ");

            foreach (var general in content.Tags.General)
                sb.Append(general + ", ");

            foreach (var copyright in content.Tags.Copyright)
                sb.Append(copyright + ", ");

            foreach (var meta in content.Tags.Meta)
                sb.Append(meta + ", ");

            sb.Remove(sb.Length - 2, 2);

            return (content.HighResFile.Url, sb.ToString());
        }
    }
}
