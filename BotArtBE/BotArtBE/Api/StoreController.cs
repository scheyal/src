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

        // POST: api/Store
        // e.g. https://localhost:44342/api/Store/Musician/Create
        [HttpPost]
        [Route("Musician/CreateOrUpdate/")]
        public async Task<ActionResult> Post([FromBody] MusicianNetModel Musician)
        {
            try
            {
                MusicianMgr mm = new MusicianMgr(AppConfig);

                MusicianModel NewMusician = MusicianMgr.NetToStoreModel(Musician);

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
            MusicianModel musician = null;
            MusicianNetModel NetMusician = null;
            try
            {
                MusicianMgr mm = new MusicianMgr(AppConfig);
                musician = await mm.Find(Name);
                if(musician != null)
                {
                    NetMusician = MusicianMgr.StoreToNetModel(musician);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Error: Musician search failed: {e.Message}");
                throw e;
            }

            return NetMusician;
        }



        // GET: api/Store/5
        [HttpGet("{id}", Name = "Get")]
        public ActionResult Get(int id)
        {
            return new NotFoundResult();
        }

        // POST: api/Store
        [HttpPost]
        public ActionResult Post(string value)
        {
            return new NotFoundResult();
        }

        // PUT: api/Store/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] string value)
        {
            return new NotFoundResult();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            return new NotFoundResult();
        }
    }
}



/** auto generate sample, remove later **
        // GET: api/Api
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Api/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Api
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Api/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
** **/
