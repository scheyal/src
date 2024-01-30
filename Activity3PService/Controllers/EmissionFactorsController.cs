using System;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using Activity3PService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;

#pragma warning disable CS8601, CS8600, CS8602 // Possible null reference assignment.
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

                    if (LibHdr == null)
                    {
                        LibResponse.SetStatus(StatusCodes.Status404NotFound, $"Cannot find library: {Name}");
                    }

                    LibResponse.SetStatus(StatusCodes.Status200OK, "OK");

                    LibResponse.Library = Globals.Libraries.Find(lib => lib.Header.Id == LibHdr.Id);


                }

            }
            catch (Exception ex)
            {
                LibResponse.SetStatus(500, $"Internal Server Error: {ex.Message}");
            }

            return LibResponse;

        }

        [HttpGet(Name = "GetFactor")]
        public LibraryResponseModel GetFactor(string LibraryId, string FactorName)
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

                if (String.IsNullOrEmpty(FactorName) || 
                    String.IsNullOrEmpty(LibraryId) || 
                    Globals.LibraryHeaders == null )
                {
                    throw new Exception("Cannot find library or factor.");
                }

                LibraryModel libModel = new LibraryModel();

                libModel = Globals.Libraries.Find(lib => lib.Header.Id == LibraryId);

                if (libModel == null)
                {
                    throw new Exception($"Cannot get library {LibraryId}");
                }
                LibResponse.Library.Header = libModel.Header;

                List<Factor>? LibFactors = libModel.Factors;
                if(LibFactors != null && LibFactors.Count == 0)
                {
                    throw new Exception("Library is missing factors.");

                }

                Factor factor = LibFactors.Find(f => f.Name == FactorName);
                if( factor == null )
                {
                    throw new Exception($"Cannot find factor {FactorName}");
                }

                List<Factor> factors = new List<Factor>();
                factors.Add(factor);
                // libModel.Header is already set from initial find. Now add the library.
                LibResponse.Library = libModel;
                LibResponse.Library.Factors = factors;
                LibResponse.SetStatus(StatusCodes.Status200OK, "OK");

            }
            catch (Exception ex)
            {
                LibResponse.SetStatus(500, $"Internal Server Error: {ex.Message}");
            }

            return LibResponse;

        }

    }
#pragma warning restore CS8601, CS8600, CS8602 // Possible null reference assignment.

}
