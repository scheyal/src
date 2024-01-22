using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Activity3PService.Pages
{
    public class IndexModel : PageModel
    {
        public string ServerURL { get; set; }
        public string TestURL { get; set; }
        public IndexModel()
        {
            TestURL = ServerURL = "[Unavailable]";

        }
        public void OnGet()
        {
            ServerURL = HttpContext.Request.GetEncodedUrl();
            TestURL = ServerURL + "api/Test";
        }
    }
}
