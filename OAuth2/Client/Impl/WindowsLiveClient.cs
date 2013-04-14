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
                request.AddParameter("access_token", (object)this.AccessToken, ParameterType.GetOrPost);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsLiveClient"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="configuration">The configuration.</param>
        public WindowsLiveClient(IRequestFactory factory, IClientConfiguration configuration) 
            : base(factory, configuration)
        {
        }

        /// <summary>
        /// Defines URI of service which issues access code.
        /// </summary>
        protected override Endpoint AccessCodeServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://login.live.com",
                    Resource = "/oauth20_authorize.srf"
                };
            }
        }

        /// <summary>
        /// Defines URI of service which issues access token.
        /// </summary>
        protected override Endpoint AccessTokenServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://login.live.com",
                    Resource = "/oauth20_token.srf"
                };
            }
        }

        /// <summary>
        /// Defines URI of service which allows to obtain information about user which is currently logged in.
        /// </summary>
        protected override Endpoint UserInfoServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://apis.live.net/v5.0",
                    Resource = "/me"
                };
            }
        }

        protected override IAuthenticator GetAuthenticator(Oauth2AccessToken accessToken)
        {
            return new WindowsLiveOAuth2UriQueryParameterAuthenticator(accessToken.Token);
        }
        
        /// <summary>
        /// Should return parsed <see cref="UserInfo"/> from content received from third-party service.
        /// </summary>
        /// <param name="content">The content which is received from third-party service.</param>
        protected override UserInfo ParseUserInfo(string content)
        {
            var response = JObject.Parse(content);
            return new UserInfo
            {
                Id = response["id"].Value<string>(),
                FirstName = response["first_name"].Value<string>(),
                LastName = response["last_name"].Value<string>(),
                Email = response["emails"]["preferred"].Value<string>(),
                PhotoUri = string.Format("https://cid-{0}.users.storage.live.com/users/0x{0}/myprofile/expressionprofile/profilephoto:Win8Static,UserTileSmall,UserTileStatic/MeControlXXLUserTile?ck=2&ex=24", response["id"].Value<string>())
            };
        }

        public override string ProviderName
        {
            get { return "WindowsLive"; }
        }
    }
}