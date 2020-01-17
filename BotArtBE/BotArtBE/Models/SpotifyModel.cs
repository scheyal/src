using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace BotArtBE.Models
{
    public class SpotifyModel
    {
        const string OpenSpotifyArtistUrl = "https://open.spotify.com/artist/";

        private IConfiguration AppConfig;
        public string ClientId { get; set; }
        public string Secret { get; set; }


        public SpotifyModel(IConfiguration configuration)
        {
            AppConfig = configuration;
            ClientId = AppConfig["SpotifyClientId"];
            Secret = AppConfig["SpotifyClientSecret"];
        }

        private async Task<SpotifyWebAPI> GetSpotifyWebAPIClient()
        {
            CredentialsAuth auth = new CredentialsAuth(ClientId, Secret);
            Token token = await auth.GetToken();
            SpotifyWebAPI api = new SpotifyWebAPI() { TokenType = token.TokenType, AccessToken = token.AccessToken };
            return api;
        }

        public async Task<FullArtist> GetFullArtist(string Name)
        {
            FullArtist artist = null;
            // Get music api client
            SpotifyWebAPI api = await GetSpotifyWebAPIClient();

            // Search library
            SearchItem results = await api.SearchItemsEscapedAsync(Name, SearchType.Artist, 1);
            if(results.Artists.Total <= 0)
            {
                // not found - return none
                artist = null;
            }
            else
            {
                artist = results.Artists.Items[0];
            }
            return artist;
        }

        public async Task<string> GetArtistUri(string Name)
        {
            FullArtist artist = await GetFullArtist(Name);
            return artist?.Uri;
        }

        public async Task<string> GetArtistURL(string Name)
        {
            string Url = null;
            string Uri = await GetArtistUri(Name);
            if(!String.IsNullOrEmpty(Uri))
            {
                //split into
                string[] uriParts = Uri.Split(":");
                Url = OpenSpotifyArtistUrl + uriParts[2];
            }
            return Url;
        }
    }
}
