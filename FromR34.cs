using AnimePicturesScroller.Models;
using DiscordRandomPicBot.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Text;

namespace DiscordRandomPicBot
{
    public class FromR34
    {
        const string TAGS_URL = "https://api.r34.app/booru/rule34.xxx/tags?baseEndpoint=rule34.xxx&order=count&limit=10";
        const string BASE_URL = "https://api.r34.app/booru/rule34.xxx/posts?baseEndpoint=rule34.xxx&limit=20";


        private static Dictionary<string, string> Tags = new()
        {
            { "-yaoi", "-yaoi" },
            { "-furry", "%7C-furry" },
            { "-ai_generated", "%7C-ai_generated" },
            { "-koikatsu", "%7C-koikatsu" },
            { "-futanari", "%7C-futanari" },
            { "-alien", "%7C-alien" },
            { "-horse_penis", "%7C-horse_penis" },
            { "-male/male", "%7C-male/male" },
            { "-male_penetrating_male", "%7C-male_penetrating_male" },
            { "-gay", "%7C-gay" },
            { "-gay_sex", "%7C-gay_sex" },
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
                Tags.Add(tag, $"%7C{tag}");
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
            if (Tags.First().Value.StartsWith("%7C"))
                Tags[Tags.First().Key] = Tags.First().Value.Remove(0, 3);
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
            string animeImagesData = "";
            HttpClient client = new HttpClient();
            Console.WriteLine(BASE_URL + $"&pageID={pageId}" + $"&tags={GenerateTagsRequest()}");
            HttpResponseMessage response = await client.GetAsync(BASE_URL + $"&pageID={pageId}" + $"&tags={GenerateTagsRequest()}");
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

        public static async Task<(Uri, string)> RandomNSFW()
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
