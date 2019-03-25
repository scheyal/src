using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;



namespace SimpleKeyVault.Pages
{
    public class IndexModel : PageModel
    {
        public IConfiguration AppConfig { get; set; }
        public IndexModel(IConfiguration appconfig)
        {
            AppConfig = appconfig;
        }

        public void OnGet()
        {

        }
    }
}
