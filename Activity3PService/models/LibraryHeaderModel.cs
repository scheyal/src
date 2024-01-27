using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;

namespace Activity3PService.Models
{
    public class LibraryHeaderModel
    {

        // Library properties
        public string Name { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public string LatestVersion { get; set; }
        public string  Id { get; set; }
        public string Authority { get; set; }
        public string SupportURL { get; set; }


        public LibraryHeaderModel()
        {

            Name = "Uninitialized";
            Version = String.Empty;
            LatestVersion = String.Empty;
            Type = String.Empty;
            Id = String.Empty;
            Authority = String.Empty;
            SupportURL = String.Empty;

        }

    }
}
