using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// LinkedIn authentication client.
    /// </summary>
    public class LinkedInClient : OAuthClient
    {
        public static string ClientName = "LinkedIn";

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
            Resource = "/v1/people/~:(id,first-name,last-name,picture-url)"
        };

        public static UserInfo UserInfoParserFunc(string content)
        {
            throw new System.NotImplementedException();
        }

        public LinkedInClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, RequestTokenEndpoint, LoginEndpoint, TokenEndpoint,
                   UserInfoEndpoint, factory, configuration, UserInfoParserFunc)
        {
        }
    }
}