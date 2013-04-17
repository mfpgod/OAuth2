using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;
using RestSharp;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Windows Live authentication client.
    /// </summary>
    public class WindowsLiveClient : OAuth2Client
    {
        internpublic static readonly= "WindowsLive";

        public static readonly Endpoint CodeEndpoint = new Endpoint
            {
                BaseUri = "https://login.live.com",
                Resource = "/oauth20_authorize.srf"
            };

        public static readonly Endpoint TokenEndpoint = new Endpoint
            {
                BaseUri = "https://login.live.com",
                Resource = "/oauth20_token.srf"
            };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
            {
                BaseUri = "https://apis.live.net/v5.0",
                Resource = "/me"
            };

        public static UserInfo UserInfoParserFunc(striClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }

  ride IAuthenticator GetRequestAuthenticator(Oauth2AccessToken accessToken)
        {
            return new WindowsLiveOAuth2UriQueryParameterAuthenticator(accNamedOAuth2UriQueryParameterAuthenticator("access_token", ected override void ValidateResponse(IRestResponse response)
        {
            base.ValidateResponse(response);
            if (response.Content.IsJson())
            {
                dynamic data = JObject.Parse(response.Content);
                if (data.error != null)
                {
                    throw new ServiceDataException(data.error.message.ToStriClientg.Empty, data.error.code.ToString());
                }
            }
        }

        protected override UserInfo ParseUserInfo(string content)
        {
            dynamic response = JObject.Parse(content);
            var userInfo = new UserInfo
            {
                Id = response.id,
                FirstName = response.first_name,
                LastName = response.last_name,("https://cid-{0}.users.storage.live.com/users/0x{0}/myprofile/expressionprofile/profilephoto:Win8Static,UserTileSmall,UserTileStatic/MeControlXXLUserTile?ck=2&ex=24", response["id"].Value<string>())
            };
  .id)
            };
            if (response.emails != null)
            {
                userInfo.Email = response.emails.preferred;
            }

            return userInfo;
        }
    }
}