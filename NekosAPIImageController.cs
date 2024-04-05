using AnimePicturesScroller.Models;
using Newtonsoft.Json;

namespace DiscordRandomPicBot
{
    public class NekosAPIImageController
    {
        private const string IMAGES_API_URL = "https://api.nekosapi.com/v3/images";
        private const string ARTIST_API_URL = "https://api.nekosapi.com/v3/artists";
        private const string CHARACTER_API_URL = "https://api.nekosapi.com/v3/characters";

        //[HttpGet]
        public static async Task<ImageRandomResult> RandomAnimeImage()
        {
            var photo = await GetRandomAnimeImage();
            return photo;
        }

        private static async Task<ImageRandomResult> GetRandomAnimeImage()
        {
            string animeImagesData = "";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(IMAGES_API_URL + "/random?limit=1");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                animeImagesData = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<ImageRandomResult>(animeImagesData);
        }
    }
}
