using System.Collections.Specialized;

using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// GitHub authentication client.
    /// </summary>
    public class GitHubClient : OAuth2Client
    {
        public static stringreadonlyg ClientName = "GitHub";

        public static readonly Endpoint CodeEndpoint = new Endpoint
            {
                BaseUri = "https://github.com",
                Resource = "/login/oauth/authorize"
            };

        public static readonly Endpoint TokenEndpoint = new Endpoint
            {
                BaseUri = "https://github.com",
                Resource = "/login/oauth/access_token"
            };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
            {
                BaseUri = "https://api.github.com/",
                Resource = "/user"
            };

        public static UserInfo UserInfoPaientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }
)
        {
        }

        protected override UserInfo ParseUserInfo(string content)
        {
            dynamic response = JObject.Parse(content);

            var name = response.name.ToString();
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
                Id = response.id,
                Email = response.email,
                PhotoUri = response.avatar_url,
                FirstName = firstName,
                LastName = lastName
            };
        }
cessTokenExchangeObject(NameValueCollection parameters, IClientConfiguration configuration)
        {
            return new
            {
                code = parameters["code"],
                client_id = configuration.ClientId,
                client_secret = configuration.ClientSecret,
                redirect_uri = configuration.RedirectUri,
                state = parameters["state"]
            };
        }
    }
}
