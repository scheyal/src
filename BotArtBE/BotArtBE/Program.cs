using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;


namespace BotArtBE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            
            .ConfigureAppConfiguration((ctx, builder) =>
            {
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(AuthenicateKeyVault));
                builder.AddAzureKeyVault(
                    Constants.KeyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
            })
            .UseStartup<Startup>();


        private static async Task<string> AuthenicateKeyVault(string authority, string resource, string scope)
        {
            System.Diagnostics.Trace.TraceInformation("Callback: AuthenicateKeyVault");
            System.Diagnostics.Trace.TraceInformation($"Params: authority={authority}, resource={resource}, scope={scope}");

            string result = string.Empty;
            try
            {
                AzureServiceTokenProvider TokenProvider = new AzureServiceTokenProvider();
                result = await TokenProvider.KeyVaultTokenCallback(authority, resource, scope);
                if (TokenProvider.PrincipalUsed != null)
                {
                    string MSIName = TokenProvider.PrincipalUsed.ToString();
                    System.Diagnostics.Trace.TraceInformation($"MSIName = {MSIName}");
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceInformation($"Exception: Cannot access keyvault ({e.Message})");

            }
            return result;

        }


    }
}
