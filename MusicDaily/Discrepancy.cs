using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MusicDaily.SpotifyPlaylist;
using MusicDaily.CosmosDBDocument;
using SendGrid.Helpers.Mail;

namespace MusicDaily
{
    public static class Discrepancy
    {
        [FunctionName("Discrepancy")]
        public static void Run(
            [ServiceBusTrigger("daily", Connection = "ServiceBusConnection")]CosmosPlaylist NewPlaylist,
            [CosmosDB(
                databaseName: "DailyMusic",
                collectionName: "Playlists",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "Select * from Playlists")] IEnumerable<CosmosPlaylist> OldPlaylists,
            [CosmosDB(
                databaseName: "DailyMusic",
                collectionName: "Playlists",
                ConnectionStringSetting = "CosmosDBConnection")] out dynamic document,
            [SendGrid] out SendGridMessage message,
            ILogger log)
        {
            CosmosPlaylist OldPlaylist = isExistingDocument(NewPlaylist, OldPlaylists);
            message = null;
            // If the Spotify Playlist does not exists in the CosmosDB, then create it
            if (OldPlaylist == null)
            {
                log.LogInformation("New CosmsoDB Document Detected");
                document = Newtonsoft.Json.JsonConvert.SerializeObject(NewPlaylist);
            } else // Otherwise, determine if the CosmosDB Playlist is up-to-date
            {
                HashSet<CosmosTrack> OldTracks = new HashSet<CosmosTrack>(OldPlaylist.tracks);
                HashSet<CosmosTrack> NewTracks = new HashSet<CosmosTrack>(NewPlaylist.tracks);
                
                var tracks_added = TracksAdded(OldTracks, NewTracks);
                var tracks_removed = TracksRemoved(OldTracks, NewTracks);

                // Log the Tracks that have been Added and Deleted Since the Last Scan
                foreach (var track in tracks_added)
                {
                    log.LogInformation($"Track Added: {track.name} from '{OldPlaylist.name}'");
                }
                foreach (var track in tracks_removed)
                {
                    log.LogInformation($"Track Removed: {track.name} from '{OldPlaylist.name}'");
                }

                // No document is created if the playlist has not changed
                if (tracks_added.Count == 0 && tracks_removed.Count == 0)
                {
                    document = null;
                } else // Update the existing document sends an email
                {
                    if (tracks_added.Count > 0)
                    {
                        // Construct the Email
                        string message_content = $"The Spotify Playlist &ndash; <b>{NewPlaylist.name}</b> &ndash; has recently been updated with new tracks:<br><br>";
                        foreach (var track in tracks_added)
                        {
                            message_content += track.ToString();
                        }
                        message = new SendGridMessage();
                        message.AddTo("clothiernamedjeremiah@gmail.com");
                        message.SetFrom(new EmailAddress("dailyspotifymusic@gmail.com"));
                        message.AddContent("text/html", message_content);
                        message.SetSubject($"Music Daily {DateTime.Today.ToString("MM-dd-yyyy")}");
                    }
                    document = NewPlaylist; // Update the existing document
                }



            }
        }

        public static CosmosPlaylist isExistingDocument(CosmosPlaylist NewPlaylist, IEnumerable<CosmosPlaylist> OldPlaylists)
        {
            CosmosPlaylist OldPlaylist = null;
            foreach (var item in OldPlaylists)
            {
                if (NewPlaylist.playlist_id == item.playlist_id)
                {
                    OldPlaylist = item;
                    break;
                }
            }
            return OldPlaylist;
        }

        /*
         * Set Theory:
         * Let A be the set of old tracks in a playlist and let B be the set of new tracks in a playlist.
         * Then, the set of 'tracks added' can be represented as the following expression: B \ (A ∩ B)
         */
        public static HashSet<CosmosTrack> TracksAdded(HashSet<CosmosTrack> old_tracks, HashSet<CosmosTrack> new_tracks)
        {
            // Methods on HashSet modify the current HashSet. I create copies so that I don't modify the parameters
            HashSet<CosmosTrack> intersection = new HashSet<CosmosTrack>(old_tracks);
            HashSet<CosmosTrack> tracks_added = new HashSet<CosmosTrack>(new_tracks);

            intersection.IntersectWith(new_tracks);
            tracks_added.ExceptWith(intersection);
            return tracks_added;
        }
        /*
         * Set Theory:
         * Let A be the set of old tracks in a playlist and let B be the set of new tracks in a playlist.
         * Then, the set of 'tracks added' can be represented as the following expression: A \ (A ∩ B)
         */
        public static HashSet<CosmosTrack> TracksRemoved(HashSet<CosmosTrack> old_tracks, HashSet<CosmosTrack> new_tracks)
        {
            // Methods on HashSet modify the current HashSet. I create copies so that I don't modify the parameters
            HashSet<CosmosTrack> intersection = new HashSet<CosmosTrack>(old_tracks);
            HashSet<CosmosTrack> tracks_removed = new HashSet<CosmosTrack>(old_tracks);

            intersection.IntersectWith(new_tracks);
            tracks_removed.ExceptWith(intersection);
            return tracks_removed;
        }
    }

    
}
