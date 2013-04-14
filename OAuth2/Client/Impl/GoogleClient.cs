using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Google authentication client.
    /// </summary>
    public class GoogleClient : OAuth2Client
    {
        public GoogleClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(factory, configuration)
        {
        }

        public override string ProviderName
        {
            get { return "Google"; }
        }

        protected override Endpoint AccessCodeServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://accounts.google.com",
                    Resource = "/o/oauth2/auth"
                };
            }
        }

        protected override Endpoint AccessTokenServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://accounts.google.com",
                    Resource = "/o/oauth2/token"
                };
            }
        }

        protected override Endpoint UserInfoServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://www.googleapis.com",
                    Resource = "/oauth2/v1/userinfo"
                };
            }
        }

        protected override UserInfo ParseUserInfo(string content)
        {
            var response = JObject.Parse(content);
            return new UserInfo
            {
                Id = response["id"].Value<string>(),
                Email = response["email"].Value<string>(),
                FirstName = response["given_name"].Value<string>(),
                LastName = response["family_name"].Value<string>(),
                PhotoUri = response["picture"].SafeGet(x => x.Value<string>())
            };
        }
    }
}