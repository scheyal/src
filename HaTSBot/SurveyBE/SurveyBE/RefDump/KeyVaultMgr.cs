using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace LFS.Azure
{
     public static class KeyVaultMgr
    {
        public static string ClientID { get; set; }
        public static string ClientCertThumb { get; set; }
        public static string KeyUrl { get; set; }

        public static ClientAssertionCertificate ClientCert { get; set; }

        static KeyVaultMgr()
        {
            ClientCert = null;
            ClientID = String.Empty;
            ClientCertThumb = String.Empty;
            KeyUrl = String.Empty;
        }

        public static void Initialize(string clientId, string clientCertThumb, string keyUrl)
        {
            ClientID = clientId;
            ClientCertThumb = clientCertThumb;
            KeyUrl = keyUrl;
        }
        public static string StorageKey { get; set; }

        public static async void FetchStroageKey()
        {
            GetCert();
            StorageKey = await GetStorageKey();
        }
        private static async Task<string> GetStorageKey()
        {
            System.Diagnostics.Trace.TraceInformation("Call: KeyVaultMgr::GetStorageKey");

            KeyVaultClient kvCli = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        (authority, resource, scope) => AuthenicateKeyVault(authority, resource, scope)));
            var result = await kvCli.GetSecretAsync(KeyUrl);
            return result.Value;
        }

        /// <summary>
        /// Get authentication token using application password secret.
        /// </summary>
        /// <param name="authority"></param>
        /// <param name="resource"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private static async Task<string> AuthenicateKeyVault(string authority, string resource, string scope)
        {
            System.Diagnostics.Trace.TraceInformation("Callback!: KeyVaultMgr::AuthenicateKeyVault");

            // return await GetAccessToken(authority, resource, scope); 

            /* DEPRECATE * Violation: Using secret in source code */
            // const string AppId = "0f4a3f53-fd43-4c20-8232-7ac1ab236bfb";
            const string ClientSecret = "u2j2eLkvSYJ75a96c5z5oEi859nyrFLO/icFjixvksc="; // name = KVAccess
            var clientCredential = new ClientCredential(ClientID, ClientSecret);
            var authenticationContext = new AuthenticationContext(authority);
            var result = await authenticationContext.AcquireTokenAsync(resource, clientCredential);
            return result.AccessToken;
            /* */

        }

        /// <summary>
        /// Get authentication token using certificate (preferred)
        /// </summary>
        /// <param name="authority"></param>
        /// <param name="resource"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            System.Diagnostics.Trace.TraceInformation("Callback!: KeyVaultMgr::GetAccessToken using cert");

            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
             var result = await context.AcquireTokenAsync(resource, ClientCert);
            return result.AccessToken;
        }

        public static X509Certificate2 FindCertificateByThumbprint(string findValue)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection col = 
                    store.Certificates.Find(X509FindType.FindByThumbprint, findValue, false); // Don't validate certs, since the test root isn't installed.
                if (col == null || col.Count == 0)
                    return null;
                return col[0];
            }
            finally
            {
                store.Close();
            }
        }

        public static void GetCert()
        {
            if (ClientCert == null)
            {
                var clientAssertionCertPfx = FindCertificateByThumbprint(ClientCertThumb);
                ClientCert = new ClientAssertionCertificate(ClientID, clientAssertionCertPfx);
            }
        }

    }
}