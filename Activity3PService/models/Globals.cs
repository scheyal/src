using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace Activity3PService.Models
{
    public static class Globals
    {
        public struct Creds
        {
            public string Username;
            public string Password;
            public string Hash;
        }

        public static Creds Test;

        // Known Activity Types
        public const string PurchasedElectricity = "Purchased Electricity";


        static Globals()
        {
            Test = new Creds();
        }

        public static void LazyInitializer(IConfiguration config)
        {            
            var U = config.GetValue<string>("Authentication:TestUser");
            var P = config.GetValue<string>("Authentication:TestPassword");
            Test.Username = U ?? "Bad Configuration: Cannot find test user";
            Test.Password = P ?? "!@34@!$#%asdfHHJm";
            Test.Hash = ComputeSha256Hash($"{U}:{P}");
        }

        static public string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static public string GetToken(string Username, string Password)
        {
            return ComputeSha256Hash($"{Username}:{Password}");
        }

        static public bool VerifyToken(string token)
        {
            return token == Globals.Test.Hash;
        }
    }
}
