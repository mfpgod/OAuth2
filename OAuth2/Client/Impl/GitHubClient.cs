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
        public GitHubClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(factory, configuration)
        {
        }

        pride string ProviderName
        {
            get { return "GitHub"; }
        }

        /// <summaoverride Endpoint AccessCodeServiceEndpoint
        {
            get { return new Endpoint { BaseUri = "https://github.com", Resource = "/login/oauth/authorize" }; }
        }

        /// <summaoverride Endpoint AccessTokenServiceEndpoint
        {
            get { return new Endpoint { BaseUri = "https://github.com", Resource = "/login/oauth/access_token" }; }
        }

        /// <summaoverride Endpoint UserInfoServiceEndpoint
        {
            get { return new Endpoint { BaseUri = "https://api.github.com/", Resource = "/user" }; }
        }        
 

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

        protected override UserInfo ParseUserInfo(string content)
        {
            var cnt = JObject.Parse(content);

            var names = cnt["name"].Value<string>().Split(' ').ToList();
            return new UserInfo
            {
                Email = cnt["email"].Value<string>(),
                ProviderName = this.ProviderName,
                PhotoUri = cnt["avatar_url"].Value<string>(),
                Id = cnt["id"].Value<string>(),
                FirstName = names.Count > 0 ? names.First() : cnt["login"].Value<string>(),
                LastName = names.Count > 1 ? names.Last() : string.Empty,
            };
        }

    }
}
