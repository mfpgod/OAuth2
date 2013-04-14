using System.Linq;
using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;
using RestSharp;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// MailRu authentication client.
    /// </summary>
    public class MailRuClient : OAuth2Client
    {
        internal class MailRuOAuth2UriQueryParameterAuthenticator : OAuth2Authenticator
        {
            private readonly IClientConfiguration _clientConfiguration;

            public MailRuOAuth2UriQueryParameterAuthenticator(string accessToken, IClientConfiguration clientConfiguration)
                : base(accessToken)
            {
                _clientConfiguration = clientConfiguration;
            }

            public override void Authenticate(IRestClient client, IRestRequest request)
            {
                // Source documents
                // http://api.mail.ru/docs/guides/restapi/
                // http://api.mail.ru/docs/reference/rest/users.getInfo/

                request.AddParameter("app_id", _clientConfiguration.ClientId);
                request.AddParameter("method", "users.getInfo");
                request.AddParameter("secure", "1");
                request.AddParameter("session_key", AccessToken);

                //sign=hex_md5('app_id={client_id}method=users.getInfosecure=1session_key={access_token}{secret_key}')
                string signature = string.Concat(request.Parameters.OrderBy(x => x.Name).Select(x => string.Format("{0}={1}", x.Name, x.Value)).ToList());
                signature = (signature + _clientConfiguration.ClientSecret).GetMd5Hash();

                request.AddParameter("sig", signature); 
            }
        }

        public static string ClientName = "MailRu";

        public static readonly Endpoint CodeEndpoint = new Endpoint
            {
                BaseUri = "https://connect.mail.ru",
                Resource = "/oauth/authorize"
            };

        public static readonly Endpoint TokenEndpoint = new Endpoint
            {
                BaseUri = "https://connect.mail.ru",
                Resource = "/oauth/token"
            };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
            {
                BaseUri = "http://www.appsmail.ru",
                Resource = "/platform/api"
            };

        public static UserInfo UserInfoParserFunc(string content)
        {
            var response = JArray.Parse(content);
            return new UserInfo
            {
                ProviderName = ClientName,
                Id = response[0]["uid"].Value<string>(),
                FirstName = response[0]["first_name"].Value<string>(),
                LastName = response[0]["last_name"].Value<string>(),
                Email = response[0]["email"].Value<string>(),
                PhotoUri = response[0]["pic"].Value<string>()
            };
        }

        public MailRuClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }

        protected override IAuthenticator GetRequestAuthenticator(Oauth2AccessToken accessToken)
        {
            return new MailRuOAuth2UriQueryParameterAuthenticator(accessToken.Token, ClientConfiguration);
        }
    }
}