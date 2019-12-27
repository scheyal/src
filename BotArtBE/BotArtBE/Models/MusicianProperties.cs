using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BotArtBE.Models
{
    public class Album
    {
        public string Name { get; set; }
        public int Votes { get; set; }
        public List<string> Users { get; set; }
    }

    public class Song
    {
        public string Name { get; set; }
        public int Votes { get; set; }
        public List<string> Users { get; set; }
    }

    public class MusicianProperties
    {
        public List<Album> FavoriteAlbums { get; set; }
        public List<Song> FavoriteSongs { get; set; }
        public List<string> Reviews { get; set; }

        public MusicianProperties()
        {
            FavoriteAlbums = new List<Album>();
            FavoriteSongs = new List<Song>();
            Reviews = new List<string>();
        }
    }

    public class MusicianPropertiesMgr
    {
        public MusicianProperties Properties { get; set; }
        public string Serialized { get; set; }

        public MusicianPropertiesMgr(MusicianProperties props = null)
        {
            Properties = props;
        }

        public void Deserialize()
        {
            Properties = MusicianPropertiesMgr.Deserialize(Serialized);
        }

        public void Serialize()
        {
            Serialized = MusicianPropertiesMgr.Serialize(Properties);
        }

        public static string Serialize(MusicianProperties props)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string jsonString = JsonSerializer.Serialize(props, options);
            return jsonString;
        }

        public static MusicianProperties Deserialize(string jsonString)
        {
            MusicianProperties props = JsonSerializer.Deserialize<MusicianProperties>(jsonString);
            return props;
        }


    }
}
