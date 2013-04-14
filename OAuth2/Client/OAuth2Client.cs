using System;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;
using RestSharp;
using RestSharp.Contrib;
using RestSharp.Validation;

namespace OAuth2.Client
{
    /// <summary>
    /// Base class for OAuth2 client implementation.
    /// </summary>
    public abstract class OAuth2Client : IClient
    {
        private const string AccessTokenKey = "access_token";

        private readonly IRequestFactory _factory;
        private readonly IClientConfiguration _configuration;

        /// <summary>
        /// Defines URI of service which issues access code.
        /// </summary>
        protected abstract Endpoint AccessCodeServiceEndpoint { get; }

        /// <summary>
        /// Defines URI of service which issues access token.
        /// </summary>
        protected abstract Endpoint AccessTokenServiceEndpoint { get; }

        /// <summary>
        /// Defines URI of service which allows to obtain information about user 
        /// who is currently logged in.
        /// </summary>
        protected abstract Endpoint UserInfoServiceEndpoint { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Client"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="configuration">The configuration.</param>
        protected OAuth2Client(IRequestFactory factory, IClientConfiguration configuration)
        {
            _factory = factory;
            _configuration = configuration;
        }

        #region ICLient impl

        public abstract string ProviderName { get; }

        public string GetLoginLinkUri(string state = null)
        {
            var client = _factory.NewClient();
            client.BaseUrl = AccessCodeServiceEndpoint.BaseUri;

            var request = _factory.NewRequest();
            request.Resource = AccessCodeServiceEndpoint.Resource;

            request.AddObject(this.BuildRequestTokenExchangeObject(_configuration, state));

            return client.BuildUri(request).ToString();
        }

        public virtual OauthAccessToken Finalize(NameValueCollection parameters)
        {
            if (!parameters["error"].IsEmpty())
                throw new Exception(parameters["error"]);

            var client = _factory.NewClient();
            client.BaseUrl = AccessTokenServiceEndpoint.BaseUri;

            var request = _factory.NewRequest();
            request.Resource = AccessTokenServiceEndpoint.Resource;
            request.Method = Method.POST;
            request.AddObject(this.BuildAccessTokenExchangeObject(parameters, _configuration));

            var response = client.Execute(request);
            string accessToken;
            try
            {
                // response can be sent in JSON format
                accessToken = (string) JObject.Parse(response.Content).SelectToken(AccessTokenKey);
            }
            catch (JsonReaderException)
            {
                // or it can be in "query string" format (param1=val1&param2=val2)
                accessToken = HttpUtility.ParseQueryString(response.Content)[AccessTokenKey];
            }
            return new Oauth2AccessToken(accessToken);
        }

        public IRestResponse GetData(OauthAccessToken accessToken, string resource)
        {
            var oauth2AccessToken = accessToken as Oauth2AccessToken;
            Require.Argument("accessToken", oauth2AccessToken);

            var client = _factory.NewClient();
            client.BaseUrl = UserInfoServiceEndpoint.BaseUri;
            client.Authenticator = GetAuthenticator(oauth2AccessToken);

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

        protected virtual IAuthenticator GetAuthenticator(Oauth2AccessToken accessToken)
        {
            return new OAuth2UriQueryParameterAuthenticator(accessToken.Token);
        }

        protected virtual dynamic BuildRequestTokenExchangeObject(IClientConfiguration configuration,
                                                                  string state = null)
        {
            return new
                {
                    response_type = "code",
                    client_id = _configuration.ClientId,
                    redirect_uri = _configuration.RedirectUri,
                    scope = _configuration.Scope,
                    state
                };
        }

        protected virtual dynamic BuildAccessTokenExchangeObject(NameValueCollection parameters,
                                                                 IClientConfiguration configuration)
        {
            return new
                {
                    code = parameters["code"],
                    client_id = configuration.ClientId,
                    client_secret = configuration.ClientSecret,
                    redirect_uri = configuration.RedirectUri,
                    grant_type = "authorization_code"
                };
        }

        /// <summary>
        /// Should return parsed <see cref="UserInfo"/> using content received from provider.
        /// </summary>
        /// <param name="content">The content which is received from provider.</param>
        protected abstract UserInfo ParseUserInfo(string content);

        #endregion
    }
}