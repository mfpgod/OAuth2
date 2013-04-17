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
        public static ing ClientName = "LinkedInOauth2";

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
            
            //data error response
            //{"errorCode": 0,"message": "Unknown field {_gdfgdfgfirst-name} in resource {Person}","requestId": "W42QCQM6KP","status": 400,"timestamp": 1366198167795}
            if (data.errorCode != null)
            {
                throw new ClientException(data.message.ToString(), data.status.ToString(), data.errorCode.ToString());
            }
            //access token error response
            //{"error":"invalid_request","error_description":"missing required parameters, includes an invalid parameter value, parameter more then once. : client_id"}
            if (data.error != null)
            {
                throw new ClientException(data.error.ToString(), null, data.error_description.ToString()); BuildAccessTokenExchangeObject(NameValueCollIAuthenticator GetRequestAuthenticator(Oauth2AccessToken accessToken)
        {
            return new LinkedInOAuth2NamedOAuth2UriQueryParameterAuthenticator("oauth2_access_token", ken);ildAccessTokenExchangeObject(NameValueCollection parameters, IClientConfiguration configuration)
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
                ProviderName = ientName,
                Id = response.id.ToString(),
                FirstName = response.firstName,
                LastName = response.lastName,
                PhotoUri = response.pictureUrl
            };

            return user;
        }
    }
}            return user;
        }
    }
}