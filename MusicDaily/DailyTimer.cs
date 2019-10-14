using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using MusicDaily.SpotifyPlaylist;
using MusicDaily.CosmosDBDocument;
using System.Net;


namespace MusicDaily
{
    public static class DailyTimer
    {
        private static readonly string CLIENT_ID = GetEnvironmentVariable("SpotifyClientId");
        private static readonly string CLIENT_SECRET = GetEnvironmentVariable("SpotfiyClientSecret");

        private static readonly HttpClient client = new HttpClient();


        public static Dictionary<string, string> PLAYLIST_IDS = new Dictionary<string, string>(){
            { "Daily Lift", "37i9dQZF1DWU13kKnk03AP" },
            { "Love Pop", "37i9dQZF1DX50QitC6Oqtn" },
            { "Now Hear This", "4ANVDtJVtVMVc2Nk79VU1M" },
            { "Dance Hits", "37i9dQZF1DX0BcQWzuB7ZO" },
            { "Feeling Accomplished", "37i9dQZF1DWTDafB3skWPN" },
            { "Testing 123", "1NZMnU4H5m5iYPF0RGvluO"}
        };

        [FunctionName("DailyTimer")]
        public static async Task Run(
            [TimerTrigger("*/30 * * * * *")]TimerInfo myTimer,
            [ServiceBus("additions",Connection = "ServiceBusConnection")] IAsyncCollector<CosmosPlaylist> playlistQueue,
            ILogger log)
        {
            // Acquire Access Token
            AccessToken token = GetToken().Result;
            log.LogInformation($"Token: {token.access_token}");
            log.LogInformation($"Access Token Acquired and will expires in {token.expires_in / 60} minutes.");

            foreach(var id in PLAYLIST_IDS)
            {
                log.LogInformation($"Fetching Playlist '{id.Key}'");
                Playlist playlist = GetPlaylist(token, id.Value).Result;
                if (playlist != null)
                {
                    // Clean the data before adding it to the Queue. This reduces the size of the message sent to the queue
                    var cosmos_formatted_playlist = formatToCosmosDBDocument(playlist);
                    string next = playlist.tracks.next;
                    // Spotify API will limit each query to a maximium of 100 tracks. If a playlist has more
                    // then 100 tracks we will need to make several API calls using the 'next' attribute
                    while (next != null)
                    {
                        Tracks tracks = GetTracks(token, next).Result;
                        cosmos_formatted_playlist.AppendTracks(tracks);
                        next = tracks.next;
                    }

                   /* 
                    * Abstracts the Queue Binding, It will also serialize the object to JSON before putting 
                    * it on the Queue. After we've received the function call request, we're sending it to 
                    * the Queue for us, it will even create the Queue on the demand.
                    */
                    await playlistQueue.AddAsync(cosmos_formatted_playlist);

                } else
                {
                    log.LogInformation("playlist not found, invalid playlist id");
                }
            }
        }
        // Reference: https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library#environment-variables
        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }

        //Reference: https://developer.spotify.com/documentation/general/guides/authorization-guide/#client-credentials-flow
        public static async Task<AccessToken> GetToken()
        {
            //Define Headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{CLIENT_ID}:{CLIENT_SECRET}")));

            //Prepare Request Body
            List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
            requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

            //Request Token
            var request = await client.PostAsync("https://accounts.spotify.com/api/token", requestBody);
            var response = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AccessToken>(response);
        }

        public static async Task<Playlist> GetPlaylist(AccessToken token, string spotify_id)
        {
            // Define Headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{token.access_token}");

            //API Request
            var request = await client.GetAsync($"https://api.spotify.com/v1/playlists/{spotify_id}");
            var response = await request.Content.ReadAsStringAsync(); // TODO: how to send a fail message and stop job
            if (request.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<Playlist>(response);
        }

        public static async Task<Tracks> GetTracks(AccessToken token, string next)
        {
            // Define Headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{token.access_token}");

            //API Request
            var request = await client.GetAsync(next);
            var response = await request.Content.ReadAsStringAsync(); // TODO: how to send a fail message and stop job
            if (request.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<Tracks>(response);
        }
        public static CosmosPlaylist formatToCosmosDBDocument(Playlist playlist)
        {
            var tracks = new List<CosmosTrack>();
            foreach (var item in playlist.tracks.items)
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
                tracks.Add(track);
            }
            
            var cosmos_playlist = new CosmosPlaylist(
                playlist.id,
                playlist.name,
                playlist.description,
                playlist.followers.total,
                tracks,
                playlist.uri
                );
            return cosmos_playlist;
        }
    }
}