using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;
using RestSharp;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Facebook authentication client.
    /// </summary>
    public class FacebookClient : OAuth2Client
    {
        public static readonly string ClientName = "Facebook";

        public static readonly Endpoint CodeEndpoint = new Endpoint
            {
                BaseUri = "https://www.facebook.com",
                Resource = "/dialog/oauth"
            };

        public static readonly Endpoint TokenEndpoint = new Endpoint
                {
                    BaseUri = "https://graph.facebook.com",
                    Resource = "/oauth/access_token"
                };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
        {
            BaseUri = "https://graph.facebook.com",
            Resource = "/me?fields=id,first_name,last_name,email,picture"
        };

        public FacebookClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration)
        {
        }

        protected override void ValidateResponse(IRestResponse response)
        {
            base.ValidateResponse(response);
            if (response.Content.IsJson())
            {
                dynamic data = JObject.Parse(response.Content);
                if (data.error != null)
                {
                    throw new ServiceDataException(data.error.Clienttring(), data.error.type.ToString(), data.error.code.ToString());
                }
            }
        }

        protected override UserInfo ParseUserInfo(string content)
        {
            dynamic response = JObject.Parse(content);

            var user = new UserInfo
          {
                ProviderName = ClientName,
                Id = response.id.ToString(),
                FirstName = response.first_name,
                LastName = response.last_name,
                Email = response.email
            };
            if (response.pictur,
                PhotoUri = (response.picture != null && response.picture.data != null) ? response.picture.data.url : null
            };
            return user;
        }
    }
}