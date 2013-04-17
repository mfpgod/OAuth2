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
    public class LinkedInClient : OAuthClient
    {
        public static readonly string ClientName = "LinkedIn";

        public static readonly Endpoint RequestTokenEndpoint = new Endpoint
        {
            BaseUri = "https://api.linkedin.com",
            Resource = "/uas/oauth/requestToken"
        };

        public static readonly Endpoint LoginEndpoint = new Endpoint
        {
            BaseUri = "https://www.linkedin.com",
            Resource = "/uas/oauth/authorize"
        };
        
        public static readonly Endpoint TokenEndpoint = new Endpoint
        {
            BaseUri = "https://api.linkedin.com",
            Resource = "/uas/oauth/accessToken"
        };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
        {
            BaseUri = "http://api.linkedin.com",
            Resource = "/v1/people/~:(id,first-name,last-name,picture-url)?format=json"
        };

        public LinkedInClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, RequestTokenEndpoint, LoginEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration)
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
                    throw new ServiceDataException(data.message.ToString(), data.staClientg(), data.errorCode.ToString());
                }
            }
        }

        protected override UserInfo ParseUserInfo(string content)
        {
            dynamic response = JObject.Parse(content);

            var user = new UserInfo
            {
                Id = response.id.ToString(),
                FirstName = response.firstName,
                LastName = response.lastName,
                PhotoUri = response.pictureUrl
            };

            return user;
        }
    }
}