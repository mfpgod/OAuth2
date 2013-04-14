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

        private readonly IRequestFactory _factory;
        private readonly IClientConfiguration _configuration;

        private string _secret;

        /// <summary>
        /// Defines URI of service which is called for obtaining request token.
        /// </summary>
        protected abstract Endpoint RequestTokenServiceEndpoint { get; }

        /// <summary>
        /// Defines URI of service which should be called to initiate authentication process.
        /// </summary>
        protected abstract Endpoint LoginServiceEndpoint { get; }

        /// <summary>
        /// Defines URI of service which issues access token.
        /// </summary>
        protected abstract Endpoint AccessTokenServiceEndpoint { get; }

        /// <summary>
        /// Defines URI of service which is called to obtain user information.
        /// </summary>
        protected abstract Endpoint UserInfoServiceEndpoint { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthClient" /> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="configuration">The configuration.</param>
        protected OAuthClient(IRequestFactory factory, IClientConfiguration configuration)
        {
            _factory = factory;
            _configuration = configuration;
        }

        #region IClient impl

        public abstract string ProviderName { get; }

        public string GetLoginLinkUri(string state = null)
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
            client.BaseUrl = UserInfoServiceEndpoint.BaseUri;
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(
                _configuration.ClientId, _configuration.ClientSecret, oauth1AccessToken.Token, oauth1AccessToken.TokenSecret);

            var request = _factory.NewRequest();
            request.Resource = resource;

            return client.Execute(request);
        }

        public UserInfo GetUserInfo(OauthAccessToken accessToken)
        {
            var restResponse = GetData(accessToken, UserInfoServiceEndpoint.Resource);

            var userInfo = ParseUserInfo(restResponse.Content);
            userInfo.ProviderName = ProviderName;

            return userInfo;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Issues request for request token and returns result.
        /// </summary>
        private NameValueCollection GetRequestToken()
        {
            var client = _factory.NewClient();
            client.BaseUrl = RequestTokenServiceEndpoint.BaseUri;
            client.Authenticator = OAuth1Authenticator.ForRequestToken(
                _configuration.ClientId, _configuration.ClientSecret, _configuration.RedirectUri);

            var request = _factory.NewRequest();
            request.Resource = RequestTokenServiceEndpoint.Resource;
            request.Method = Method.POST;

            var response = client.Execute(request);
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

            var request = _factory.NewRequest();
            request.Resource = LoginServiceEndpoint.Resource;
            request.AddParameter(OAuthTokenKey, response[OAuthTokenKey]);
            if (!state.IsEmpty())
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
            client.Authenticator = OAuth1Authenticator.ForAccessToken(
                _configuration.ClientId, _configuration.ClientSecret, token, _secret, verifier);

            var request = _factory.NewRequest();
            request.Resource = AccessTokenServiceEndpoint.Resource;
            request.Method = Method.POST;

            var response = client.Execute(request);
            return HttpUtility.ParseQueryString(response.Content);
        }

        /// <summary>
        /// Should return parsed <see cref="UserInfo"/> using content of callback issued by service.
        /// </summary>
        protected abstract UserInfo ParseUserInfo(string content);

        #endregion
    }
}