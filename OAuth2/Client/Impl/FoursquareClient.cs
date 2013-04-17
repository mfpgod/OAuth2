using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Foursquare authentication client.
  
    /// https://developer.foursquare.com/overview/auth.html  /// </summary>
    public class FoursquareClient : OAuth2Client
    {
        public static strinreadonlyng ClientName = "Foursquare";

        public static readonly Endpoint CodeEndpoint = new Endpoint
            {
                BaseUri = "https://foursquare.com",
                Resource = "/oauth2/authorize"
            };

        public static readonly Endpoint TokenEndpoint = new Endpoint
            {
                BaseUri = "https://foursquare.com",
                Resource = "/oauth2/access_token"
            };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
            {
                BaseUri = "https://api.foursquare.com",
                Resource = "/v2/users/self"
            };

        public static UserInfo UserInfoPry factory, IClientConfiguration configuration)
            : base(ClientName, CodeEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {)
        {
        }

        protected override UserInfo ParseUserInfo(string content)
        {
            dynamic response = JObject.Parse(content);
            var user = response.response.user;
            return new UserInfo
            {
                ProviderName = ClientName,
                Id = user.id,
                FirstName = user.firstName,
                LastName = user.lastName,
                Email = (user.contact != null) ? user.contact.email : null,
                PhotoUri = user.photo
            };
        }
    }
}