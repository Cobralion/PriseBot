using Google.Cloud.Firestore;
using Infrastructure.Enities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Database
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly IConfiguration _configuration;

        public Database(IConfiguration configuration)
        {
            _configuration = configuration;
            _firestoreDb = FirestoreDb.Create(_configuration["firebase-project"]);
        }

        public async Task<GuildSettings> GetGuildSettings(string guild)
        {
            CollectionReference usersRef = _firestoreDb.Collection("servers");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if(document.Id == guild)
                {
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    return new GuildSettings(documentDictionary["Channel"] as string, documentDictionary["Minimum"] as string, documentDictionary["Maximum"] as string);
                }
            }

            return null;
        }

        public object GetMinMax(string guild)
        {
            throw new NotImplementedException();
        }

        public async Task SetGuildSettings(string guild, GuildSettings guildSettings)
        {
            DocumentReference docRef = _firestoreDb.Collection("servers").Document(guild);
            Dictionary<string, object> server = new Dictionary<string, object>
            {
                { "Channel", guildSettings.Channel },
                { "Minimum", guildSettings.Minimum },
                { "Maximum", guildSettings.Maximum },
            };

            await docRef.SetAsync(server);
        }

        public async Task<int> GetSpruecheLength()
        {
            CollectionReference internRef = _firestoreDb.Collection("intern");
            QuerySnapshot snapshot = await internRef.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Id == "sprueche")
                {
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    return Convert.ToInt32(documentDictionary["Length"] as string);
                }
            }

            return 0;
        }

        public async Task<Sprueche> GetSpruchWithIndex(int index)
        {
            CollectionReference videosRef = _firestoreDb.Collection("sprueche");
            Query query = videosRef.WhereEqualTo("Index", index);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                return new Sprueche(documentDictionary["Header"] as string, documentDictionary["Value"] as string);
            }

            return new Sprueche("", "");
        }

        public async Task<int> GetVideoLegth()
        {
            CollectionReference internRef = _firestoreDb.Collection("intern");
            QuerySnapshot snapshot = await internRef.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Id == "videos")
                {
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    return Convert.ToInt32(documentDictionary["Length"] as string);
                }
            }

            return 0;
        }

        public async Task<string> GetVideoWithIndex(int index)
        {
            CollectionReference videosRef = _firestoreDb.Collection("videos");
            Query query = videosRef.WhereEqualTo("Index", index.ToString());
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    return documentDictionary["Url"] as string;
            }

            return "";
        }
    }
}
