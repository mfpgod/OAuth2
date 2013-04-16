using System.Collections.Specialized;

using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

nausing RestSharp;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// VKontakte authentication client.
    /// </summary>
    public class VKontakteClient : OAuth2Client
    {
        public static sreadonlystring ClientName = "VKontakte";

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

        public static UserInfo UserInfoParsIClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }
)
        {
        }

        protected override Oauth2AccessToken ParseOauthAccessToken(IRestResponse response)
        {
            var token = base.ParseOauthAccessToken(response);
            token.ExtraData.Add("user_id", ((dynamic)JObject.Parse(response.Content)).user_id.ToString());
            return token;
        }

        protected override void ValidateResponse(IRestResponse response)
        {
            base.ValidateResponse(response);
            if (response.Content.IsJson())
            {
                dynamic data = JObject.Parse(response.Content);
                if (data.error != null)
                {
                    throw new ServiceDataException(data.error_msg.ToString(), string.Empty, data.error_code.ToString());
                }
            }
        }

        public override UserInfo GetUserInfo(OauthAccessToken accessToken)
        {
            var userIdParams = new NameValueCollection { { "uids", accessToken.ExtraData["user_id"] } };
            var restResponse = GetData(accessToken, AccessUserInfoEndpoint.BaseUri, AccessUserInfoEndpoint.Resource, userIdParams);

            var userInfo = this.ParseUserInfo(restResponse.Content);
            userInfo.ProviderName = Name;

            return userInfo;
        }

        protected override UserInfo ParseUserInfo(string content)
        {
            dynamicent)["response"][0];
            return new UserInfo
            {
                ProviderName = ClientName,
      Id = response.uid,
                FirstName = response.first_name,
                LastName = response.last_name,
                PhotoUri = response.photo
            };
        }
ust after obtaining response with access token from third-party service.
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