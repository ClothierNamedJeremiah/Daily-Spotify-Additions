using System;
using System.Collections.Generic;
using System.Text;

namespace MusicDaily.CosmosDBDocument
{
    public class CosmosTrack
    {
        public CosmosTrack(string track_id, string name, int popularity, string preview_url, string uri)
        {
            this.track_id = track_id;
            this.name = name;
            this.popularity = popularity;
            this.preview_url = preview_url;
            this.uri = uri;
        }

        public override bool Equals(object obj)
        {
            CosmosTrack other = obj as CosmosTrack;
            return (other != null)
                && (track_id == other.track_id);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + track_id.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            if (preview_url == null)
            {
                return $"Track: {name}<br>Popularity: {popularity}<br><br>";
            } else
            {
                return $"Track: {name}<br>Popularity: {popularity}<br>Preview (30 sec): <a href=\"{preview_url}\">listen</a><br><br>";
            }
            
        }

        public string track_id { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public string preview_url { get; set; }
        public string uri {get; set;}
        public List<string> artists { get; set; }
    }
}
