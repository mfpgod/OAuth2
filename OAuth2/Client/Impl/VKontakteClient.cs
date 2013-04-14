using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// VKontakte authentication client.
    /// </summary>
    public class VKontakteClient : OAuth2Client
    {
        public static string ClientName = "VKontakte";

        public static readonly Endpoint CodeEndpoint = new Endpoint
        {
            BaseUri = "http://oauth.vk.com",
            Resource = "/authorize"
        };

        public static readonly Endpoint TokenEndpoint = new Endpoint
        {
            BaseUri = "https://oauth.vk.com",
            Resource = "/access_token"
        };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
        {
            BaseUri = "https://api.vk.com",
            Resource = "/method/users.get?fields=uid,first_name,last_name,photo"
        };

        public static UserInfo UserInfoParserFunc(string content)
        {
            var response = JObject.Parse(content)["response"][0];
            return new UserInfo
            {
                ProviderName = ClientName,
                FirstName = response["first_name"].Value<string>(),
                LastName = response["last_name"].Value<string>(),
                Id = response["uid"].Value<string>(),
                PhotoUri = response["photo"].Value<string>()
            };
        }

        public VKontakteClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }

        /// <summary>
        /// Called just after obtaining response with access token from third-party service.
        /// Allows to read extra data returned along with access token.
        /// </summary>
//        protected override void AfterGetAccessToken(IRestResponse response)
//        {
//            _userId = JObject.Parse(response.Content)["user_id"].Value<string>();
//        }

        /// <summary>
        /// Called just before issuing request to third-party service when everything is ready.
        /// Allows to add extra parameters to request or do any other needed preparations.
        /// </summary>
//        protected override void BeforeGetUserInfo(IRestRequest request)
//        {
//            request.AddParameter("uids", _userId);
//        }
    }
}