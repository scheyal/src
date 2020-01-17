using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage;

namespace BotArtBE.Models
{
    public class MusicianMgr
    {
        private IConfiguration AppConfig;
        private readonly string TableName = "Musicians";
        private string ConnStr;
        private TableStorageAsync<MusicianModel> Table;

        public MusicianMgr(IConfiguration configuration)
        {
            AppConfig = configuration;
            ConnStr = AppConfig["StorageConnectionString"];
            Table = new TableStorageAsync<MusicianModel>(TableName, ConnStr);
        }

        public string NormalizeName(string name)
        {
            return name.ToLowerInvariant();
        }
        public async Task CreateOrUpdate(MusicianModel Musician)
        {
            try
            {
                //validate musician
                MusicianModel found = await Find(Musician.NormalizedName);
                if(found != null)
                {
                    // fail create
                    System.Diagnostics.Trace.TraceInformation($"Trying to create existing musician. Updating instead {Musician.Name}.");
                    await Update(Musician, found);
                    return;
                }

                Musician.PartitionKey = Constants.DefaultPartition;
                Musician.RowKey = Guid.NewGuid().ToString();
                await Table.ConnectAsync();
                await Table.CreateEntityAsync(Musician);

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Musician.Create Error: {e.Message}");
                throw e;
            }
        }

        public async Task<MusicianModel> Find(string Name)
        {
            MusicianModel musician = null;
            // search in store
            try
            {
                await Table.ConnectAsync();

                List<MusicianModel> musicians = (List<MusicianModel>) await Table.GetEntitiesByPropertyAsync("NormalizedName", Name);
                if (musicians.Count == 1)
                {
                    musician = musicians.First<MusicianModel>();
                }
                if (musicians.Count > 1)
                {
                    throw new Exception($"Error: found > 1 musicians named ${Name}.");
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Musician.Find Error: {e.Message}");
                throw e;

            }
            return musician;
        }

        public async Task Update(MusicianModel Musician, MusicianModel found = null)
        {
            MusicianModel dbMusician = null;
            // search in store
            try
            {
                string Name = Musician.NormalizedName;
                await Table.ConnectAsync();

                if(found != null)
                {
                    dbMusician = found;
                }
                else
                {
                    List<MusicianModel> musicians = (List<MusicianModel>)await Table.GetEntitiesByPropertyAsync("NormalizedName", Name);
                    if (musicians.Count == 1)
                    {
                        dbMusician = musicians.First<MusicianModel>();
                    }
                    else
                    {
                        throw new Exception($"Error: found > 1 musicians named ${Name}.");
                    }
                }

                Merge(Musician, dbMusician);
                dbMusician.Votes++;
                await Table.InsertOrUpdateAsync(dbMusician);

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Musician.Find Error: {e.Message}");
                throw e;
            }
        }

        private void Merge(MusicianModel MSrc, MusicianModel MDst)
        {
            if(MSrc.NormalizedName != MDst.NormalizedName)
            {
                throw new Exception($"Cannot merge {MSrc.NormalizedName} into {MDst.NormalizedName}");
            }

            MusicianProperties dp = String.IsNullOrEmpty(MDst.Properties)? new MusicianProperties() : MusicianPropertiesMgr.Deserialize(MDst.Properties);
            MusicianProperties sp = String.IsNullOrEmpty(MSrc.Properties) ? new MusicianProperties() : MusicianPropertiesMgr.Deserialize(MSrc.Properties);

            if(dp.Reviews == null)
            {
                dp.Reviews = new List<string>();
            }
            if(sp.Reviews != null)
            {
                foreach (string s in sp.Reviews)
                {
                    dp.Reviews.Add(s);
                }
            }

            if (dp.FavoriteAlbums == null)
            {
                dp.FavoriteAlbums = new List<Album>();
            }
            if(sp.FavoriteAlbums != null)
            {
                foreach (Album sa in sp.FavoriteAlbums)
                {
                    Album da = dp.FavoriteAlbums.Find(a => a.Name == sa.Name);
                    if (da == null)
                    {
                        // add album
                        Album newAlbum = new Album();
                        newAlbum.Name = sa.Name;
                        newAlbum.Votes = 1;
                        newAlbum.Users = new List<string>();
                        newAlbum.Users.Add(sa.Users.First());
                        dp.FavoriteAlbums.Add(newAlbum);
                    }
                    else
                    {
                        da.Votes++;
                        string newUser = sa.Users.First();
                        bool fUser = da.Users.Exists(x => x == newUser);
                        if (!fUser)
                        {
                            da.Users.Add(newUser);
                        }
                    }
                }
            }

            if(dp.FavoriteSongs == null)
            {
                dp.FavoriteSongs = new List<Song>();
            }

            if (sp.FavoriteSongs != null)
            {
                foreach (Song sso in sp.FavoriteSongs)
                {
                    Song ds = dp.FavoriteSongs.Find(s => s.Name == sso.Name);
                    if (ds == null)
                    {
                        // add songs
                        Song newSong = new Song();
                        newSong.Name = sso.Name;
                        newSong.Votes = 1;
                        newSong.Users = new List<string>();
                        newSong.Users.Add(sso.Users.First());
                        dp.FavoriteSongs.Add(newSong);
                    }
                    else
                    {
                        ds.Votes++;
                        string newUser = sso.Users.First();
                        bool fUser = ds.Users.Exists(x => x == newUser);
                        if (!fUser)
                        {
                            ds.Users.Add(newUser);
                        }
                    }
                }
            }

            string dstprops = MusicianPropertiesMgr.Serialize(dp);

            MDst.Properties = dstprops;
        }


        public async Task<MusicianListNetModel> GetMusicians()
        {
            MusicianListNetModel MusiciansList = null;
            // search in store
            try
            {

                await Table.ConnectAsync();
                List<MusicianModel> musicians = (List<MusicianModel>)await Table.GetAllEntitiesAsync();
                MusiciansList = new MusicianListNetModel();
                foreach(MusicianModel m in musicians)
                {
                    MusiciansList.Musicians.Add(new MusicianVotes(m.Name, m.Votes));

                }
                MusiciansList.Musicians.Sort();
                return MusiciansList;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Musician listing.Find Error: {e.Message}");
                throw e;
            }
        }

        public static bool CheckSkip(string val)
        {
            if (0 == String.Compare(val, "*n/a", true) ||
                0 == String.Compare(val, "*skip", true))
            {
                return true;
            }
            return false;
        }

        public static MusicianModel NetToStoreModel(MusicianNetModel inM)
        {
            MusicianModel outM = new MusicianModel();

            outM.Name = inM.Name;
            outM.Votes = inM.Votes;
            if (inM.Properties.FavoriteAlbums != null &&
               inM.Properties.FavoriteAlbums.Count > 0)
            {
                List<Album> newAlbums = new List<Album>();
                foreach (Album a in inM.Properties.FavoriteAlbums)
                {
                    if(!CheckSkip(a.Name))
                    {
                        a.Users = new List<string>();
                        a.Users.Add(inM.Submitter);
                        newAlbums.Add(a);
                    }
                }
                inM.Properties.FavoriteAlbums = newAlbums;
            }
            if (inM.Properties.FavoriteSongs != null &&
               inM.Properties.FavoriteSongs.Count > 0)
            {
                List<Song> newSongs = new List<Song>();
                foreach(Song s in inM.Properties.FavoriteSongs)
                {
                    if (!CheckSkip(s.Name))
                    {
                        s.Users = new List<string>();
                        s.Users.Add(inM.Submitter);
                        newSongs.Add(s);
                    }
                }
                inM.Properties.FavoriteSongs = newSongs;
            }
            if (inM.Properties.Reviews != null &&
               inM.Properties.Reviews.Count > 0)
            {
                List<string> newReviews = new List<string>();
                foreach (string r in inM.Properties.Reviews)
                {
                    if (!CheckSkip(r))
                    {
                        newReviews.Add(r);
                    }
                }
                inM.Properties.Reviews= newReviews;
            }
                outM.Properties = MusicianPropertiesMgr.Serialize(inM.Properties);
            return outM;
        }

        public static MusicianNetModel StoreToNetModel(MusicianModel inM)
        {
            MusicianNetModel outM = new MusicianNetModel();

            outM.Name = inM.Name;
            outM.Votes = inM.Votes;
            outM.Properties = MusicianPropertiesMgr.Deserialize(inM.Properties);
            outM.Submitter = "BotArtFE";
            return outM;
        }

    }
}
