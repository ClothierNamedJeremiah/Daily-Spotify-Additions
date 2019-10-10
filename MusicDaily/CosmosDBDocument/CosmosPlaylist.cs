using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using MusicDaily.SpotifyPlaylist;

namespace MusicDaily.CosmosDBDocument
{
    public class CosmosPlaylist
    {
        public CosmosPlaylist(string playlist_id, string name, string description, int followers, List<CosmosTrack> tracks, string uri)
        {
            this.playlist_id = playlist_id;
            this.name = name;
            this.description = description;
            this.followers = followers;
            this.tracks = tracks;
            this.uri = uri;
        }

        public void AppendTracks(Tracks tracks)
        {
            foreach (var item in tracks.items)
            {
                var track = new CosmosTrack(
                    item.track.id,
                    item.track.name,
                    item.track.popularity,
                    item.track.preview_url,
                    item.track.uri
                    );
                var artists = new List<string>();
                foreach (var artist in item.track.artists)
                {
                    artists.Add(artist.name);
                }
                track.artists = artists;
                this.tracks.Add(track); // Append new tracks to existing object
            }
        }

        public override string ToString()
        {
            return $"Playlist `{name} has the following additions:\n{tracks}";
        }
        [JsonProperty("id")]
        public string playlist_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int followers { get; set; }
        public List<CosmosTrack> tracks { get; set; }
        public string uri { get; set; }
    }
}
