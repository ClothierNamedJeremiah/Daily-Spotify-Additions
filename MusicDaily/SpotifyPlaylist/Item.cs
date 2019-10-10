using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicDaily.SpotifyPlaylist
{
    public class Item
    {
        public string added_at { get; set; }
        public AddedBy added_by { get; set; }
        public bool is_local { get; set; }
        public Track track { get; set; }
    }
}
