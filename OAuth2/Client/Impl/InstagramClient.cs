using System.Linq;
using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

nausing RestSharp;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Instagram authentication client.
    /// </summary>
    public class InstagramClient : OAuth2Client
    {
        public static string CreadonlyClientName = "Instagram";

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
     v1/users/{0}/lic static UserInfo UserInfoParsClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }
 )
        {
        }

        proteverride UserInfo GetUserInfo(OauthAccessToken accessToken)
        {
            var resource = AccessUserInfoEndpoint.Resource.Fill(accessToken.ExtraData["user_id"]);
            var restResponse = GetData(accessToken, AccessUserInfoEndpoint.BaseUri, resource);

            var userInfo = this.ParseUserInfo(restResponse.Content);
            userInfo.ProviderName = Name;

            return userInfo;
        }

        protected override IAuthenticator Getvoid ValidateResponse(IRestResponse response)
        {
            base.ValidateResponse(response);
            dynamic data = JObject.Parse(response.Content);

            //data error response
            //{"meta":{"error_type":"OAuthParameterException","code":400,"error_message":"\"client_id\" or \"access_token\" URL parameter missing. This OAuth request requires either a \"client_id\" or \"access_token\" URL parameter."}}
            if (data.meta != null && data.meta.error_message != null)
            {
                throw new ClientException(data.meta.error_message, null, data.meta.code.ToString());
            }

            //access token error response
            //{"code": 400, "error_type": "OAuthException", "error_message": "You must provide a client_id"}
            if (data.error_message != null)
            {
                throw new ClientException(data.error_message.ToString(), data.code.ToString());
            }        }

        protected override UserOauth2AccessToken ParseOauthAccessToken(IRestResponse response)
        {
            var token = base.ParseOauthAccessToken(response);
            token.ExtraData.Add("user_id", ((dynamic)JObject.Parse(response.Content)).user.id.ToString());
            return token;
        }

        public oIAuthenticator GetRequestAuthenticator(Oauth2AccessToken accessToken)
        {
            return new NamedOAuth2UriQueryParameterAuthenticator("access_token", accessToken.Token);        }

        protected override UserInfo ParseUserInfo           var response = JObject.Parse(contedynamic response = JObject.Parse(content);
            var user = response.data ?? response.user;
            var userInfo = new UserInfo
            {
                Id = user.id,
                PhotoUri = user.profile_picture
            };
            userInfo.FillNamesFromString((string)user.full_name.ToString());
            return userInfo;
        }
    }
}