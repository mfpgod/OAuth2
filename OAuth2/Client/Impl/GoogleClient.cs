using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Google authentication client.
    /// </summary>
    public class GoogleClient : OAuth2Client
    {
        public static stringreadonlyg ClientName = "Google";

        public static readonly Endpoint CodeEndpoint = new Endpoint
                {
                    BaseUri = "https://accounts.google.com",
                    Resource = "/o/oauth2/auth"
                };

        public static readonly Endpoint TokenEndpoint = new Endpoint
                {
                    BaseUri = "https://accounts.google.com",
                    Resource = "/o/oauth2/token"
                };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
                {
                    BaseUri = "https://www.googleapis.com",
                    Resource = "/oauth2/v1/userinfo"
                };

        public static UserInfo UserInfoPalientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        })
        {
        }

        protected override UserInfo ParseUserInfo(string content)
        {
            dynamicntent);
            return new UserInfo
            {
                ProviderName = ClientName,
                Id = response["id"].Value<string>(),
        .id,
                Email = response.email,
                FirstName = response.given_name,
                LastName = response.family_name,
                PhotoUri = response.picture
            };
        }
    }
}