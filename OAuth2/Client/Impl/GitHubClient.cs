using System.Collections.Specialized;
using System.Linq;
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
        public static string ClientName = "GitHub";

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

        public static UserInfo UserInfoParserFunc(string content)
        {
            var cnt = JObject.Parse(content);ar names = cnt["name"].Value<string>().Split(' ').ToList();
            return new UserInfo
            {
                Email = cnt["emailProviderName = ClientName,
                Email = cnt["email"].Value<string>()hotoUri = cnt["avatar_url"].Value<string>(),
                Id = cnt["id"].Value<string>(),
                FirstName = names.Count > 0 ? names.First() : cnt["login"].Value<string>(),
                LastName = names.Count > 1 ? names.Last() : string.Empty,
            };
        }

    }
}


        public GitHubClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {        
 

        protected override dynamic BuildAccessTokenExchangeObject(NameValueCollection parameters, IClientConfiguration configuration)
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

        p    }
}
