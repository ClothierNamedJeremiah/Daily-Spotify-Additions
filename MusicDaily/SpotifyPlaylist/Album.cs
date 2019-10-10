using System;
using System.Collections.Generic;
using System.Text;

namespace MusicDaily.SpotifyPlaylist
{
    public class Album
    {
        public string album_type { get; set; }
        public List<object> available_markets { get; set; }
        public ExternalUrls4 external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image2> images { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }
}
