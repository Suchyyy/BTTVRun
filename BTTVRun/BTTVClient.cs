#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BTTVRun
{
    public class BTTVClient
    {
        private const string ApiUrl = "https://api.betterttv.net/3/emotes/shared";
        public const string ResourceUrl = "https://cdn.betterttv.net/emote";

        private readonly HttpClient _client = new();

        public async Task<EmoteNode[]> Search(string? query)
        {
            var trim = query?.Trim();
            var response = await (string.IsNullOrWhiteSpace(trim) || trim.Length < 3
                ? _client.GetAsync($"{ApiUrl}/top?offset=0&limit=50")
                : _client.GetAsync($"{ApiUrl}/search?offset=0&limit=50&query={query}"));

            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();

            return (string.IsNullOrWhiteSpace(trim) || trim.Length < 3
                    ? JsonSerializer.Deserialize<List<TopNode>>(stringResponse)
                        .Select(node => node.emote)
                    : JsonSerializer.Deserialize<List<EmoteNode>>(stringResponse))
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