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

        protected override UserOauth2AccessToken ParseOauthAccessToken(IRestResponse response)
        {
            var token = base.ParseOauthAccessToken(response);
            token.ExtraData.Add("user_id", ((dynamic)JObject.Parse(response.Content)).user.id.ToString());
            return token;
        }

        public override UserInfo GetUserInfo(OauthAccessToken accessToken)
        {
            var resource = AccessUserInfoEndpoint.Resource.Fill(accessToken.ExtraData["user_id"]);
            var restResponse = GetData(accessToken, AccessUserInfoEndpoint.BaseUri, resource);

            var userInfo = this.ParseUserInfo(restResponse.Content);
            userInfo.ProviderName = Name;

            return userInfo;
        }

        protected override IAuthenticator GetRequestAuthenticator(Oauth2AccessToken accessToken)
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