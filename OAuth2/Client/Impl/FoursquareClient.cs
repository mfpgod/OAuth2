using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// Foursquare authentication client.
    /// </summary>
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
            var response = JObject.Parse(content);
            return new UserInfo
            {
                ProviderName = ClientName,
                Id = response["response"]["user"]["id"].Value<string>(),
                FirstName = response["response"]["user"]["firstName"].Value<string>(), response["response"]["user"]["lastName"].Value<string>(),
                Email = re
                Email = response["response"]["user"]["contact"]["email"].Value<string>(),
                PhotoUri = response["response"]["user"]["photo"].Value<string>()
            };>
        /// Called just before issuing request to third-party service when everything is ready.
        /// Allows to add extra parameters to request or do any other needed preparations.
        /// </summary>
//        protected override void BeforeGetUserInfo(IRestRequest request)
//        {
//            // Source document 
//            // https://developer.foursquare.com/overview/auth.html
//        }
    }
}