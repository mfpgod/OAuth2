using System.Collections.Specialized;

using Newtonsoft.Json.Linq;

using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

using RestSharp;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// LinkedIn authentication client.
    /// </summary>
    public class LinkedInOauth2Client : OAuth2Client
    {
        public static internal class LinkedInOAuth2UriQueryParameterAuthenticator : OAuth2Authenticator
        {
            public LinkedInOAuth2UriQueryParameterAuthenticator(string accessToken)
                : base(accessToken)
            {
            }

            public override void Authenticate(IRestClient client, IRestRequest request)
            {
                request.AddParameter("oauth2_access_token", this.AccessToken, ParameterType.GetOrPost);
            }
        }
ic static readonly string ClientName = "LinkedInOauth2";

        public static readonly Endpoint CodeEndpoint = new Endpoint
        {
            BaseUri = "https://www.linkedin.com",
            Resource = "/uas/oauth2/authorization"
        };

        public static readonly Endpoint TokenEndpoint = new Endpoint
        {
            BaseUri = "https://www.linkedin.com",
            Resource = "/uas/oauth2/accessToken"
        };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
        {
            BaseUri = "https://api.linkedin.com",
            Resource = "/v1/people/~:(id,first-name,last-name,picture-url)?format=json"
        };

        public LinkedInOauth2Client(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration)
        {
        }

        protected override void ValidateResponse(IRestResponse response)
        {
            base.ValidateResponse(response);
            if (response.Content.IsJson())
            {
                dynamic data = JObject.Parse(response.Content);
                if (data.errorCode != null)
                {
                    throw new ServiceDataException(data.message.ToString(), data.status.ToString(), data.errorCode.ToString());
                }
            }
        }

        protected override dynamic BuildAccessTokenExchangeObject(NameValueCollIAuthenticator GetRequestAuthenticator(Oauth2AccessToken accessToken)
        {
            return new LinkedInOAuth2UriQueryParameterAuthenticator(accessToken.Token);ildAccessTokenExchangeObject(NameValueCollection parameters, IClientConfiguration configuration)
        {
            return new
            {
                code = parameters["code"],
                client_id = configuration.ClientId,
                client_secret = configuration.ClientSecret,
                redirect_uri = configuration.RedirectUri,
                grant_type = "authorization_code",
                state = parameters["state"]
            };
        }

        protected override UserInfo ParseUserInfo(string content)
        {
            dynamic response = JObject.Parse(content);

            var user = new UserInfo
            {
                ProviderName = ClientName,
                Id = response.id.ToString(),
                FirstName = response.firstName,
                LastName = response.lastName,
                PhotoUri = response.pictureUrl
            };

            return user;
        }
    }
}