using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.Formats.Asn1;
using CsvHelper;
using System.IO;
using System.Globalization;

namespace Activity3PService.Models
{
    public static class Globals
    {
        public static IConfiguration? Config = null;
        public static IWebHostEnvironment? HostEnv = null;
        public static string ContentRoot = string.Empty;
        public struct Creds
        {
            public string Username;
            public string Password;
            public string Hash;
        }

        public static Creds Test;

        // Known Activity Types
        public const string PurchasedElectricity = "Purchased Electricity";


        static public List<LibraryHeaderModel> LibraryHeaders = new();
        static public List<LibraryModel> Libraries = new();


        static Globals()
        {
            Test = new Creds();
        }

        public static void LazyInitializer(IConfiguration config, IWebHostEnvironment env)
        {            
            var U = config.GetValue<string>("Authentication:TestUser");
            var P = config.GetValue<string>("Authentication:TestPassword");
            Test.Username = U ?? "Bad Configuration: Cannot find test user";
            Test.Password = P ?? "!@34@!$#%asdfHHJm";
            Test.Hash = ComputeSha256Hash($"{U}:{P}");
            Config = config;
            HostEnv = env;
            ContentRoot = env.ContentRootPath;

            LoadLibraries();
        }

        static public string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new();
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

        static public void LoadLibraries()
        {

            if(Config == null)
            {
                throw new Exception("Invalid Configuration");
            }
            string? libPath = Config["Libraries:Path"];
            string? libManifest = Config["Libraries:Manifest"];


            if (libPath == null || libManifest == null)
            {
                throw new Exception("Cannot find library manifest [1].");
            }

            string Manifest = Path.Combine(ContentRoot, libPath, libManifest);
            if (!File.Exists(Manifest))
            {
                throw new Exception("Cannot find library manifest [2].");
            }

            LibraryHeaders = ReadLibraryManifest(Manifest);

            foreach(var LH in LibraryHeaders)
            {
                string libId = LH.Id + ".csv";
                string libFile = Path.Combine(ContentRoot, libPath, libId);

                LibraryModel lib = new LibraryModel(LH);                    
                lib.Factors = ReadLibrary(libFile);
                Libraries.Add(lib);

            }
        }

        private static List<LibraryHeaderModel> ReadLibraryManifest(string filePath)
        {
            try
            {
                if(null == filePath)
                {
                    throw new Exception("Invalid filePath");
                }

                using (var reader = new StreamReader(filePath))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        
                        var records = csv.GetRecords<LibraryHeaderModel>().ToList();
                        return records;
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static List<Factor> ReadLibrary(string? filePath)
        {
            try
            {
                if (null == filePath)
                {
                    throw new Exception("Invalid filePath");
                }

                using (var reader = new StreamReader(filePath))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {

                        var records = csv.GetRecords<Factor>().ToList();
                        return records;
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
