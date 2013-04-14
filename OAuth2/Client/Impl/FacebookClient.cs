using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Facebook authentication client.
    /// </summary>
    public class FacebookClient : OAuth2Client
    {
        public static string ClientName = "Facebook";

        public static readonly Endpoint CodeEndpoint = new Endpoint
            {
                BaseUri = "https://www.facebook.com",
                Resource = "/dialog/oauth"
            };

        public static readonly Endpoint TokenEndpoint = new Endpoint
                {
                    BaseUri = "https://graph.facebook.com",
                    Resource = "/oauth/access_token"
                };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
        {
            BaseUri = "https://graph.facebook.com",
            Resource = "/me?fields=id,first_name,last_name,email,picture"
        };

        public static UserInfo UserInfoParserFunc(string content)
        {
            var response = JObject.Parse(content);
            return new UserInfo
            {
                ProviderName = ClientName,
                Id = response["id"].Value<string>(),
                FirstName = response["first_name"].Value<string>(),
                LastName = response["last_name"].Value<string>(),
                Email = response["email"].Value<string>(),
                PhotoUri = response["picture"]["data"]["url"].Value<string>()
            };
        }

        public FacebookClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }
    }
}