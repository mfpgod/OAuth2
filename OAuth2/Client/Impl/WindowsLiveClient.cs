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
        internal class WindowsLiveOAuth2UriQueryParameterAuthenticator : OAuth2Authenticator
        {
            public WindowsLiveOAuth2UriQueryParameterAuthenticator(string accessToken)
                : base(accessToken)
            {
            }

            public override void Authenticate(IRestClient client, IRestRequest request)
            {
                request.AddParameter("access_token", this.AccessToken, ParameterType.GetOrPost);
            }
        }

        public static string ClientName = "WindowsLive";

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

        public static UserInfo UserInfoParserFunc(string content)
        {
            var response = JObject.Parse(content);
            return new UserInfo
            {
                ProviderName = ClientName,
                Id = response["id"].Value<string>(),
                FirstName = response["first_name"].Value<string>(),
                LastName = response["last_name"].Value<string>(),
                Email = response["emails"]["preferred"].Value<string>(),
                PhotoUri = string.Format("https://cid-{0}.users.storage.live.com/users/0x{0}/myprofile/expressionprofile/profilephoto:Win8Static,UserTileSmall,UserTileStatic/MeControlXXLUserTile?ck=2&ex=24", response["id"].Value<string>())
            };
        }

        public WindowsLiveClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }

        protected override IAuthenticator GetRequestAuthenticator(Oauth2AccessToken accessToken)
        {
            return new WindowsLiveOAuth2UriQueryParameterAuthenticator(accessToken.Token);
        }
    }
}