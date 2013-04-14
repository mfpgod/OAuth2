using System.Linq;
using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;
using RestSharp;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Odnoklassniki authentication client.
    /// </summary>
    public class OdnoklassnikiClient : OAuth2Client
    {
        internal class OdnoklassnikiOAuth2UriQueryParameterAuthenticator : OAuth2Authenticator
        {
            private readonly IClientConfiguration _clientConfiguration;

            public OdnoklassnikiOAuth2UriQueryParameterAuthenticator(string accessToken, IClientConfiguration clientConfiguration)
                : base(accessToken)
            {
                _clientConfiguration = clientConfiguration;
            }

            public override void Authenticate(IRestClient client, IRestRequest request)
            {
                // Source document
                // http://dev.odnoklassniki.ru/wiki/pages/viewpage.action?pageId=12878032

                request.AddParameter("application_key", _clientConfiguration.ClientPublic);
                request.AddParameter("method", "users.getCurrentUser");

                // Signing.
                // Call API methods using access_token instead of session_key parameter
                // Calculate every request signature parameter sig using a little bit different way described in
                // http://dev.odnoklassniki.ru/wiki/display/ok/Authentication+and+Authorization
                // sig = md5( request_params_composed_string+ md5(access_token + application_secret_key)  )
                // Don't include access_token into request_params_composed_string
                string signature = string.Concat(request.Parameters.OrderBy(x => x.Name).Select(x => string.Format("{0}={1}", x.Name, x.Value)).ToList());
                signature = (signature + (AccessToken + _clientConfiguration.ClientSecret).GetMd5Hash()).GetMd5Hash();

                request.AddParameter("access_token", AccessToken);
                request.AddParameter("sig", signature);
            }
        }

        public static string ClientName = "Odnoklassniki";
        
        public static readonly Endpoint CodeEndpoint = new Endpoint
        {
            BaseUri = "http://www.odnoklassniki.ru",
            Resource = "/oauth/authorize"
        };

        public static readonly Endpoint TokenEndpoint = new Endpoint
        {
            BaseUri = "http://api.odnoklassniki.ru",
            Resource = "/oauth/token.do"
        };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
        {
            BaseUri = "http://api.odnoklassniki.ru",
            Resource = "/fb.do"
        };

        public static UserInfo UserInfoParserFunc(string content)
        {
            var response = JObject.Parse(content);
            return new UserInfo
            {
                ProviderName = ClientName,
                Id = response["uid"].Value<string>(),
                FirstName = response["first_name"].Value<string>(),
                LastName = response["last_name"].Value<string>(),
                PhotoUri = response["pic_1"].Value<string>()
            };
        }

        public OdnoklassnikiClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }

        protected override IAuthenticator GetRequestAuthenticator(Oauth2AccessToken accessToken)
        {
            return new OdnoklassnikiOAuth2UriQueryParameterAuthenticator(accessToken.Token, ClientConfiguration);
        }
    }
}