using DiscordRandomPicBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRandomPicBot
{
    public class FromTenor
    {
        private const string _apiKey = "AIzaSyAcICFpdD9Fgl00gN7HT9IR461kzdzJuyY";
        private const string _clientName = "DiscordBotTenorGifs";

        private const string _apiUrl = "https://tenor.googleapis.com/v2/";

        private static List<string> _popularSearches = new List<string>()
        {
            "anime",
            "fate stay night",
            "genshin impact",
            "lain",
            "dota 2",
            "i hate you",
            "fate grand order",
            "escape from tarkov",
            "adzumanga daiyo",
            "funny cat",
            "frieren",
            "rule",
        };

        public static async Task<TenorGifResult> SearchContent(string search = "")
        {
            if (string.IsNullOrEmpty(search))
            {
                search = _popularSearches[new Random().Next(0, _popularSearches.Count - 1)];
            }
            string data = "";
            var searchUrl = _apiUrl + "search?q=" + search + "&key=" + _apiKey + "&client_key=" + _clientName + "&limit=" + 50;

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(searchUrl);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await content.ReadAsStringAsync();
            }

            var json = JsonConvert.DeserializeObject<TenorGifResult>(data);

            return json;
        }

        public static string RandomGif(TenorGifResult gifs)
        {
            int count = gifs.Gifs.Length;

            return gifs.Gifs[new Random().Next(0, count - 1)].MediaFormats["gif"].Url;
        }

    }
}
