using System.Linq;
using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Instagram authentication client.
    /// </summary>
    public class InstagramClient : OAuth2Client
    {
        public static string ClientName = "Instagram";

        public static readonly Endpoint CodeEndpoint = new Endpoint
        {
            BaseUri = "https://api.instagram.com",
            Resource = "/oauth/authorize"
        };

        public static readonly Endpoint TokenEndpoint = new Endpoint
        {
            BaseUri = "https://api.instagram.com",
            Resource = "/oauth/access_token"
        };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
        {
            BaseUri = "https://api.instagram.com",
            Resource = "/oauth/access_token"
        };

        public static UserInfo UserInfoParserFunc(string content)
        {
            var response = JObject.Parse(content);
            var names = response["user"]["full_name"].Value<string>().Split(' ');
            return new UserInfo
            {
                ProviderName = ClientName,
                Id = response["user"]["id"].Value<string>(),
                FirstName = names.Any() ? names.First() : response["user"]["username"].Value<string>(),
                LastName = names.Count() > 1 ? names.Last() : string.Empty,
                PhotoUri = response["user"]["profile_picture"].Value<string>()
            };
        }

        public InstagramClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }
    }
}