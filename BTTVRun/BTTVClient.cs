#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BTTVRun
{
    public class BTTVClient
    {
        public const string ApiUrl = "https://api.betterttv.net/3/emotes/shared/search";
        public const string ResourceUrl = "https://cdn.betterttv.net/emote";

        private readonly HttpClient _client = new();

        public async Task<EmoteNode[]> Search(string? query)
        {
            var trim = query?.Trim();
            if (string.IsNullOrWhiteSpace(trim) || trim.Length < 3) return new EmoteNode[] { };

            var response = await _client.GetAsync($"{ApiUrl}?offset=0&limit=50&query={query}");
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<EmoteNode>>(stringResponse)
                .ToArray();
        }

        public Image DownloadImage(string id, int size)
        {
            using var stream = _client.GetStreamAsync($"{ResourceUrl}/{id}/{size}x").Result;
            return Image.FromStream(stream);
        }
        
        public BitmapImage DownloadThumbnail(EmoteNode emote)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri($"{ResourceUrl}/{emote.id}/2x.{emote.imageType}", UriKind.Absolute);
            image.EndInit();
            return image;
        }
    }
}