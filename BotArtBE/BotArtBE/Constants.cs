using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotArtBE
{
    public class Constants
    {
        public static string DefaultPartition = "BFV1";    // single partition
        public static string DefaultRow = "0";             // detects error
        public static string KeyVaultEndpoint = "https://botartvault.vault.azure.net/";
    }
}