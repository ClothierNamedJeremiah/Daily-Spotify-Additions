using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicDaily.SpotifyPlaylist
{
    // Spotify Playlist Documentation: https://developer.spotify.com/documentation/web-api/reference/playlists/get-playlist/
    public class Playlist
    {
        public bool collaborative { get; set; }
        public string description { get; set; }
        public ExternalUrls external_urls { get; set; }
        public Followers followers { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image> images { get; set; }
        public string name { get; set; }
        public Owner owner { get; set; }
        public bool @public { get; set; }
        public string snapshot_id { get; set; }
        public Tracks tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }
}
