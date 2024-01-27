using System;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using Activity3PService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Activity3PService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class EmissionFactorsController : ControllerBase
    {
        private readonly ILogger<EmissionFactorsController> Logger;
        private readonly IConfiguration Config;
        public EmissionFactorsController(ILogger<EmissionFactorsController> logger, IConfiguration configuration)
        {
            Logger = logger;
            Config = configuration;
        }

        [HttpGet(Name = "GetLibrary")]
        public LibraryResponseModel GetLibrary(string Name, string? Version)
        {
            LibraryResponseModel LibResponse = new();

            try
            {
                // Verify Authentication
                LogonModel auth = new LogonModel();
                string? authHeader = HttpContext.Request.Headers["Authorization"];
                string token = auth.ExtractHeaderToken(authHeader);
                bool Allow = auth.VerifyToken(token);

                if (!Allow)
                {
                    LibResponse.SetStatus(StatusCodes.Status401Unauthorized, "Unauthorized");
                    return LibResponse;
                }

                if(Globals.LibraryHeaders != null)
                {
                    LibraryHeaderModel? LibHdr = null;
                    
                    string predicate = $"lib => lib.Name == {Name}";
                    if(String.IsNullOrEmpty(Version))
                    {
                        LibHdr = Globals.LibraryHeaders.Find(lib => lib.Name == Name); 
                    } 
                    else
                    {
                        LibHdr = Globals.LibraryHeaders.Find(lib => lib.Name == Name && lib.Version == Version);
                    }

                    if(LibHdr != null)
                    {
                        LibResponse.SetStatus(StatusCodes.Status200OK, "OK");

                        LibResponse.Library = Globals.Libraries.Find(lib => lib.Header.Id == LibHdr.Id);
                    }
                    else
                    {
                        LibResponse.SetStatus(StatusCodes.Status404NotFound, $"Cannot find library: {Name}");
                    }

                }

            }
            catch (Exception ex)
            {
                LibResponse.SetStatus(500, $"Internal Server Error: {ex.Message}");
            }

            return LibResponse;

        }
    }
}
