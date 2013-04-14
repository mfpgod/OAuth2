using System;
using System.Collections.Specialized;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;
using RestSharp.Validation;

namespace OAuth2.Client
{
    /// <summary>
    /// Base class for OAuth (version 1) client implementation.
    /// </summary>
    public abstract class OAuthClient : IClient
    {
        private const string OAuthTokenKey = "oauth_token";
        private const string OAuthTokenSecretKey = "oauth_token_secret";
        private const string OAuthVerifierKey = "oauth_verifier";

   
        
        private string _secret;

        protected readonly IRequestFactory RequestFactory;

        protected readonly IClientConfiguration ClientConfiguration      /// <summary>
        /// Defines URI of service which is called for obtaining request token.
        /// </summary>
    
        protected Endpoint AccessRequestTokenEndpoint { get; set; }     /// <summary>
        /// Defines URI of service which is cashould be called to initiate authentication process.
        /// </summary>
        protected Endpoint AccessLoginEndpoint { get; set; }     /// <summary>
        /// Defines URI of service which is callsues access token.
        /// </summary>
        protected Endpoint AccessTokenEndpoint { get; set; }     /// <summary>
        /// Defines URI of service which is called for oto obtain user information.
        /// </summary>
        protected Endpoint AccessUserInfoEndpoint { get; set; }

        protected Func<string, UserInfo> UserInfoParser { get; set; }

        protected OAuthClient(string name, Endpoint requestTokenEndpoint, Endpoint loginEndpoint,
                               Endpoint accessTokenEndpoint, Endpoint userInfoEndpoint, IRequestFactory requestFactory,
                               IClientConfiguration clientConfiguration,
                               Func<string, UserInfo> userInfoParser)
        {
            Name = name;
            AccessRequestTokenEndpoint = requestTokenEndpoint;
            AccessLoginEndpoint = loginEndpoint;
            AccessTokenEndpoint = accessTokenEndpoint;
            AccessUserInfoEndpoint = userInfoEndpoint;

            RequestFactory = requestFactory;
            ClientConfiguration = clientConfiguration;

            UserInfoParser = userInfoParser;
        }


        #region IClient impl

        public string Name { get; protected s      public string GetLoginLinkUri(string state = null)
        {
            return GetLoginRequestUri(GetRequestToken(), state);
        }
        
        public OauthAccessToken Finalize(NameValueCollection parameters)
        {
            Require.Argument(OAuthTokenKey, parameters[OAuthTokenKey]);
            Require.Argument(OAuthVerifierKey, parameters[OAuthVerifierKey]);

            var newParameters = GetAccessToken(parameters[OAuthTokenKey], parameters[OAuthVerifierKey]);

            Require.Argument(OAuthTokenKey, newParameters[OAuthTokenKey]);
            Require.Argument(OAuthTokenSecretKey, newParameters[OAuthTokenSecretKey]);

            return new Oauth1AccessToken(newParameters[OAuthTokenKey], newParameters[OAuthTokenSecretKey]);
        }

        public IRestResponse GetData(OauthAccessToken accessToken, string resource)
        {
            var oauth1AccessToken = accessToken as Oauth1AccessToken;
            Require.Argument("accessToken", oauth1AccessToken);

            var client = _factory.NewClient();
            RequestFactory.NewClient();
            client.BaseUrl = AccessUserInfoticator.ForRequestToken(
                _configuration.ClientId, _configuratProtectedResource(
                ClientConfiguration.ClientId, ClientConfiguration.ClientSecret, oauth1AccessToken.Token, oauth1AccessToken.TokenSecret);

            var request = RequestFt.Resource = resource;

            return client.Execute(request);
        }

        public UserInfo GetUserInfo(OauthAccessToken accessToken)
        {
            var restResponse = GetData(accessToken, UserInfoServiceEndpoint.Resource);

           AccessUserInfoEndpoint.Resource);

            var userInfo = UserInfoParser(restResponse.Content);
            ndregion

        #region Private methods

        /// <summary>
        /// Issues request for request token and returns result.
        /// </summary>
        private NameValueCollection GetRequestToken()
        {
            var client = _factory.NewClient();
            client.BaseUrl = RequestTokenSRequestFactory.NewClient();
            client.BaseUrl = AccessRequestTokenticator.ForRequestToken(
                _configuration.ClientId, _configuration.ClientSecret, _configuratioClientConfiguration.ClientId, ClientConfiguration.ClientSecret, ClientConfiguration.RedirectUri);

            var request = RequestFactory.NewRequest();
            request.Resource = AccessRequestToken    var response = client.Execute(request);
            return HttpUtility.ParseQueryString(response.Content);
        }

        /// <summary>
        /// Composes login link URI.
        /// </summary>
        /// <param name="response">Content of response for request token request.</param>
        /// <param name="state">Any additional information needed by application.</param>
        private string GetLoginRequestUri(NameValueCollection response, string state = null)
        {
            if (response[OAuthTokenKey] == null)
            {
                throw new ArgumentException("{0} was not found.", OAuthTokenKey);
            }
            if (response[OAuthTokenSecretKey] == null)
            {
                throw new ArgumentException("{0} key was not found.", OAuthTokenSecretKey);
            }

            var client = _factory.NewClient();
            client.BaseUrl = LoginServiceEndpoint.BaseUri;

           RequestFactory.NewClient();
            client.BaseUrl = AccessLoginEndpoint.BaseUri;

            var request = RequestFactory.NewRequest();
            request.Resource = AccessLogin        if (!state.IsEmpty())
            {
                request.AddParameter("state", state);
            }
            _secret = response[OAuthTokenSecretKey];

            return client.BuildUri(request).ToString();
        }

        /// <summary>
        /// Obtains access token by calling corresponding service.
        /// </summary>
        /// <param name="token">Token posted with callback issued by provider.</param>
        /// <param name="verifier">Verifier posted with callback issued by provider.</param>
        /// <returns>Access token and other extra info.</returns>
        private NameValueCollection GetAccessToken(string token, string verifier)
        {
            var client = _factory.NewClient();
            client.BaseUrl = AccessTokenServiceEndpoint.BaseUri;
            client.AuthenticaRequestFactory.NewClient();
            client.BaseUrl = AccessTokenticator.ForRequestToken(
                _configuration.ClientId, _configuratAccessToken(
                ClientConfiguration.ClientId, ClientConfiguration.ClientSecret, token, _secret, verifier);

            var request = RequestFactory.NewRequest();
            request.Resource = AccessToken    var response = client.Execute(request);
            return HttpUtility.ParseQueryString(response.Content);
        }

        /// <summary>
        /// Composes login link URI.
        /// </summary>
   #endregion
    }
}