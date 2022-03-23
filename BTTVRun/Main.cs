﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
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
                var search = query.Search;
                var emotes = _client.Search(search).Result;

                return emotes.Select(emote => new Result
                {
                    Title = emote.code,
                    SubTitle = emote.user.displayName,
                    Icon = () => FetchImage(emote.id),
                    Action = e =>
                    {
                        Clipboard.SetImage(_client.DownloadImage(emote.id, 3));
                        return true;
                    }
                }).ToList();
            }
            catch (Exception e)
            {
                return new List<Result>
                {
                    new()
                    {
                        Title = e.Message
                    }
                };
            }
        }

        public void Init(PluginInitContext context)
        {
            Context = context;
        }

        private static BitmapImage FetchImage(string id)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri($"{BTTVClient.ResourceUrl}/{id}/3x", UriKind.Absolute);
            image.EndInit();
            return image;
        }
    }
}