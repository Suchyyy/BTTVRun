using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BTTVRun
{
    public class BTTVClient
    {
        public const string ApiUrl = "https://api.betterttv.net/3/emotes/shared/search";
        public const string ResourceUrl = "https://cdn.betterttv.net/emote";

        private readonly List<int> _allowedSizes = new() { 1, 2, 3 };

        private readonly HttpClient _client = new();

        public async Task<EmoteNode[]> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new EmoteNode[] { };

            var response = await _client.GetAsync($"{ApiUrl}?offset=0&limit=10&query={query}");
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<EmoteNode>>(stringResponse)
                .ToArray();
        }

        public Image DownloadImage(string id, int size)
        {
            var calcSize = _allowedSizes.Contains(size) ? size : 3;

            using var stream = _client.GetStreamAsync($"{ResourceUrl}/{id}/{calcSize}x").Result;
            return Image.FromStream(stream);
        }
    }
}