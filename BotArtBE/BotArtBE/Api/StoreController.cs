using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BotArtBE.Models;

namespace BotArtBE.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {

        private IConfiguration AppConfig;

        public StoreController(IConfiguration configuration)
        {
            AppConfig = configuration;
        }

        private void Authenticate()
        {
            string AuthKey = AppConfig["AuthKey"];
            string ClientAuthKey = Request.Headers["AuthKey"];

            if (AuthKey != ClientAuthKey)
            {
                throw new UnauthorizedAccessException("Invalid Authentication Key");
            }

        }
        // POST: api/Store
        // e.g. https://localhost:44342/api/Store/Musician/Create
        [HttpPost]
        [Route("Musician/CreateOrUpdate/")]
        public async Task<ActionResult> Post([FromBody] MusicianNetModel Musician)
        {
            Authenticate(); 
            try
            {
                MusicianMgr mm = new MusicianMgr(AppConfig);

                MusicianModel NewMusician = MusicianMgr.NetToStoreModel(Musician);
                NewMusician.NormalizedName = mm.NormalizeName(Musician.Name);

                await mm.CreateOrUpdate(NewMusician);

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Error: Musician creation failed: {e.Message}");
                return new BadRequestResult();
            }

            return new OkResult();
        }


        // Find musician
        // Example: https://localhost:44342/api/Store/Musician/Find/Led%20Zeppelin
        [HttpGet]
        [Route("Musician/Find/{Name}")]
        public async Task<MusicianNetModel> GetAsync(string Name)
        {
            Authenticate();

            MusicianModel musician = null;
            MusicianNetModel NetMusician = null;
            try
            {
                MusicianMgr mm = new MusicianMgr(AppConfig);
                string normalizedName = mm.NormalizeName(Name);
                musician = await mm.Find(normalizedName);
                if(musician != null)
                {
                    NetMusician = MusicianMgr.StoreToNetModel(musician);
                    if(Constants.UseSpotify)
                    {
                        //BUGBUG: Temp override Submitter field on the return
                        NetMusician.Submitter = await GetSpotifyURL(Name);

                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Error: Musician search failed: {e.Message}");
                throw e;
            }

            return NetMusician;
        }

        // Find musician
        // Example: https://localhost:44342/api/Store/List
        [HttpGet]
        [Route("Musician/List")]
        public async Task<MusicianListNetModel> GetAsync()
        {
            Authenticate();

            MusicianListNetModel musiciansList = null;
            try
            {
                MusicianMgr mm = new MusicianMgr(AppConfig);

                musiciansList = await mm.GetMusicians();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Error: Musician search failed: {e.Message}");
                throw e;
            }

            return musiciansList;
        }

 
        private async Task<string> GetSpotifyURL(string Name)
        {
            string URL = "N/A";
            try
            {
                SpotifyModel spotify = new SpotifyModel(AppConfig);
                URL = await spotify.GetArtistURL(Name);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Error: Musician search failed: {e.Message}");
                throw e;
            }

            return URL;
        }

    }
}
