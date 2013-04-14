using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Twitter authentication client.
    /// </summary>
    public class TwitterClient : OAuthClient
    {
        public static string ClientName = "Twitter";

        public static readonly Endpoint RequestTokenEndpoint = new Endpoint
            {
                BaseUri = "https://api.twitter.com",
                Resource = "/oauth/request_token"
            };

        public static readonly Endpoint LoginEndpoint = new Endpoint
            {
                BaseUri = "https://api.twitter.com",
                Resource = "/oauth/authenticate"
            };

        public static readonly Endpoint TokenEndpoint = new Endpoint
            {
                BaseUri = "https://api.twitter.com",
                Resource = "/oauth/access_token"
            };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
            {
                BaseUri = "https://api.twitter.com",
                Resource = "/1.1/account/verify_credentials.json"
            };

        public static UserInfo UserInfoParserFunc(string content)
        {
            var response = JObject.Parse(content);

            var name = response["name"].Value<string>();
            var index = name.IndexOf(' ');

            string firstName;
            string lastName;
            if (index == -1)
            {
                firstName = name;
                lastName = null;
            }
            else
            {
                firstName = name.Substring(0, index);
                lastName = name.Substring(index + 1);
            }

            return new UserInfo
            {
                Id = response["id"].Value<string>(),
                Email = null,
                PhotoUri = response["profile_image_url"].Value<string>(),
                FirstName = firstName,
                LastName = lastName
            };
        }

        public TwitterClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, RequestTokenEndpoint, LoginEndpoint, TokenEndpoint,
                   UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }
    }
}