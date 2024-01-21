using Microsoft.AspNetCore.Identity;
using System.Text;

namespace Activity3PService.Models
{
    public class LogonModel
    {

        public string Username { get; set; }
        public string Password { get; set; }

        public LogonModel()
        {
            Username = String.Empty;
            Password = String.Empty;

        }

        public bool Authenticate()
        {
            return Username == Globals.Test.Username && Password == Globals.Test.Password;
        }

        public string Token()
        {
            string token = Globals.GetToken(Username, Password);

            return token;
        }

        public bool VerifyToken(string token)
        {
            return Globals.VerifyToken(token);
        }

        public string ExtractHeaderToken(string? authHeader)
        {
            string token = String.Empty;

            if (authHeader != null)
            {
                authHeader = authHeader.Trim();
                string[] parts = authHeader.Split(" ");
                if (parts.Length == 2 && parts[0] == "Bearer")
                {
                    token = parts[1];
                }
            }

            return token;
        }

    }
}
