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

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, track_id) ? track_id.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, name) ? name.GetHashCode() : 0);
                return hash;
            }
        }

        public override bool Equals(object value)
        {
            CosmosTrack other = value as CosmosTrack;

            return !Object.ReferenceEquals(null, other)
                && String.Equals(track_id, other.track_id)
                && String.Equals(name, other.name);
        }

        public static bool operator ==(CosmosTrack trackA, CosmosTrack trackB)
        {
            if (Object.ReferenceEquals(trackA, trackB))
            {
                return true;
            }

            // Ensure that "numberA" isn't null
            if (Object.ReferenceEquals(null, trackA))
            {
                return false;
            }

            return (trackA.Equals(trackB));
        }

        public static bool operator !=(CosmosTrack trackA, CosmosTrack trackB)
        {
            return !(trackA == trackB);
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
