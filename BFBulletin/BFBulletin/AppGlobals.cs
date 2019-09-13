using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BFBulletin
{
    static public class AppGlobals
    {
        public static string MSIName;
        public static string KeyVaultEndpoint;

        static AppGlobals()
        {
            MSIName = "N/A";
            KeyVaultEndpoint = "https://bfbstore.vault.azure.net/";
        }
    }
}
