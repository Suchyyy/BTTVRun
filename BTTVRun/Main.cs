using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Wox.Plugin;

namespace BTTVRun
{
    public class Main : IPlugin
    {
        public string Name => "BTTVRun";
        public string Description => "BTTV Extension";
        private string IconPath => "images/icon.png";

        private PluginInitContext Context { get; set; }

        private readonly BTTVClient _client = new();

        public List<Result> Query(Query query)
        {
            try
            {
                var args = query.Search.Split(" ");
                var emotes = _client.Search(args.ElementAtOrDefault(0)).Result;

                var sizeArg = args.ElementAtOrDefault(1) ?? "3";
                var successConversion = int.TryParse(sizeArg, out var size);

                if (successConversion is false || size is > 3 or < 0)
                {
                    return new List<Result>
                    {
                        new() { Title = $"Unsupported size value: {sizeArg}. Supported size values: [1, 2, 3]." }
                    };
                }

                return emotes.Select(emote => new Result
                {
                    Title = emote.code,
                    SubTitle = $".{emote.imageType} | {emote.user.displayName}",
                    Icon = () => _client.DownloadThumbnail(emote),
                    Action = e =>
                    {
                        if (e.SpecialKeyState.CtrlPressed)
                        {
                            Clipboard.SetImage(_client.DownloadImage(emote.id, size));
                        }
                        else
                        {
                            Clipboard.SetText($"{BTTVClient.ResourceUrl}/{emote.id}/{size}x.{emote.imageType}");
                        }

                        return true;
                    }
                }).ToList();
            }
            catch (Exception e)
            {
                return new List<Result>
                {
                    new() { Title = e.Message }
                };
            }
        }

        public void Init(PluginInitContext context)
        {
            Context = context;
        }
    }
}