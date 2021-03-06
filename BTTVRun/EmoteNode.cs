namespace BTTVRun
{
    public class TopNode
    {
        public EmoteNode emote { get; set; }
        public int total { get; set; }
    }

    public class EmoteNode
    {
        public string id { get; set; }
        public string code { get; set; }
        public string imageType { get; set; }
        public UserNode user { get; set; }
    }

    public class UserNode
    {
        public string id { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
        public string providerId { get; set; }
    }
}