using System;
using System.Collections.Generic;
using System.Text;

namespace MusicDaily.SpotifyPlaylist
{
    public class Artist
    {
        public ExternalUrls5 external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }
}
