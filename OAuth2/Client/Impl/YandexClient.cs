using System.Linq;
using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Yandex authentication client.
    /// </summary>
    public class YandexClient : OAuth2Client
    {
        public static string ClientName = "Yandex";

        public static readonly Endpoint CodeEndpoint = new Endpoint
        {
            BaseUri = "https://oauth.yandex.ru",
            Resource = "/authorize"
        };

        public static readonly Endpoint TokenEndpoint = new Endpoint
        {
            BaseUri = "https://oauth.yandex.ru",
            Resource = "/token"
        };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
        {
            BaseUri = "https://login.yandex.ru",
            Resource = "/info"
        };

        public static UserInfo UserInfoParserFunc(string content)
        {
            var response = JObject.Parse(content);
            var names = response["real_name"].Value<string>().Split(' ');
            return new UserInfo
            {
                ProviderName = ClientName,
                Id = response["id"].Value<string>(),
                FirstName = names.Any() ? names.First() : response["display_name"].Value<string>(),
                LastName = names.Count() > 1 ? names.Last() : string.Empty,
                Email = response["default_email"].Value<string>(),
            };
        }

        public YandexClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }

        /// <summary>
        /// Called just before issuing request to third-party service when everything is ready.
        /// Allows to add extra parameters to request or do any other needed preparations.
        /// </summary>
        //        protected override void BeforeGetUserInfo(IRestRequest request)
        //        {
        //            // Source document 
        //            // http://api.yandex.com/oauth/doc/dg/yandex-oauth-dg.pdf
        //        }
    }
}