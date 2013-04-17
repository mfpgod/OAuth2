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

        protected OAuthClient(string name,
                              Endpoint requestTokenEndpoint,
                              Endpoint loginEndpoint,
                              Endpoint accessTokenEndpoint,
                              Endpoint userInfoEndpoint,
                              IRequestFactory requestFactory,
                              IClientConfiguration clientConfiguration)
        {
            Name = name;
            AccessRequestTokenEndpoint = requestTokenEndpoint;
            AccessLoginEndpoint = loginEndpoint;
            AccessTokenEndpoint = accessTokenEndpoint;
            AccessUserInfoEndpoint = userInfoEndpoint;

            RequestFactory = requestFactory;
            ClientConfiguration = clientConfiguration;
        }

        protected IRequestFactory RequestFactory { get; set; }

        protected IClientConfiguration ClientConfiguration { get; set; }

        /// <summary>
        /// Defines URI of service which is called for obtaining request token.
        /// </summary>
        protected Endpoint AccessRequestTokenEndpoint { get; set; }

        /// <summary>
        /// Defines URI of service which should be called to initiate authentication process.
        /// </summary>
        protected Endpoint AccessLoginEndpoint { get; set; }

        /// <summary>
        /// Defines URI of service which issues access token.
        /// </summary>
        protected Endpoint AccessTokenEndpoint { get; set; }

        /// <summary>
        /// Defines URI of service which is called to obtain user information.
        /// </summary>
        protected Endpoint AccessUserInfoEndpoint { get; set; }

        #region IClient impl

        public string Name { get; protected set; }

        public string GetLoginLinkUri(string state = null)
        {
            return GetLoginRequestUri(GetRequestToken(), state);
        }
        
        public OauthAccessToken Finalize(NameValueCollection parameters)
        {
            this.ValidateParameters(parameters);

            if (parameters[OAuthTokenKey] == null)
        ey], parameters[OAuthVerifierKey]);

            if (newParameters[OAuthTokenKey] == null)
            {
                throw new OauthException("{0} was not found.".Fill(OAuthTokenKey));
        Client    }

            if (newParameters[OAuthTokenSecretKey] == null)
            {
                throw new OauthException("{0} was not found.".Fill(OAuthTokenSecretKey));
       Client     }

            return new Oauth1AccessToken(newParameters[OAuthTokenKey], newParameters[OAuthTokenSecretKey]);
        }

        public IRestResponse GetData(OauthAccessToken accessToken, string baseUrl, string query)
        {
            var oauth1AccessToken = accessToken as Oauth1Acc, NameValueCollection extraParameters = nullessToken;
            Require.Argument("accessToken", oauth1AccessToken);

            var client = RequestFactory.NewClient();
            client.BaseUrl = baseUrl;
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(
                ClientConfiguration.ClientId, ClientConfiguration.ClientSecret, oauth1AccessToken.Token, oauth1AccessToken.TokenSecret);

            var request = RequestFactory.NewRequest();
            request.AddResourceWithQuery(query);

            var response = client.Execute(request);
            ValidateResponse(re            if (extraParameters != null)
            {
                request.AddParameters(extraParameters);
            }response);
            return response;
        }

        public UserInfo GetUserInfo(OauthAccessToken accessToken)
        {
            var restResponse = GetData(accessToken, AccessUserInfoEndpoint.BaseUri, AccessUserInfoEndpoint.Resource);
            
            var userInfo = this.ParseUserInfo(restResponse.Content);
            
            return userInfo;
        }

        #endregion

        #region Private methods

        protected virtual void ValidateParameters(NameValueCollection parameters)
        {
            if (!string.IsNullOrEmpty(parameters["oauth_problem"]))
            {
                throw new OauthException(parameters["oauth_problem"]);
            }
        }

        protected virtual void ValidateResponClientse(IRestResponse response)
        {
            if (response.ErrorException != null)
            {
                throw new ServiceDataException(response.ErrorMessage, response.ErrorException);
            }
        }

        /// <summary>
 ClientIssues request for request token and returns result.
        /// </summary>
        private NameValueCollection GetRequestToken()
        {
            var client = RequestFactory.NewClient();
            client.BaseUrl = AccessRequestTokenEndpoint.BaseUri;
            client.Authenticator = OAuth1Authenticator.ForRequestToken(
                ClientConfiguration.ClientId, ClientConfiguration.ClientSecret, ClientConfiguration.RedirectUri);

            var request = RequestFactory.NewRequest();
            request.Resource = AccessRequestTokenEndpoint.Resource;
            request.Method = Method.POST;

            var response = client.Execute(request);
            this.ValidateResponse(response);

            var parameters = HttpUtility.ParseQueryString(response.Content);
            ValidateParameters(parameters);

            return parameters;
        }

        /// <summary>
        /// Composes login link URI.
        /// </summary>
        /// <param name="parameters">Content of parameters for request token request.</param>
        /// <param name="state">Any additional information needed by application.</param>
        private string GetLoginRequestUri(NameValueCollection parameters, string state = null)
        {
            if (parameters[OAuthTokenKey] == null)
            {
                throw new OauthException("{0} was not found.".Fill(OAuthTokenKey));
            }

            if (parameters[OAuthTokenSecretKey] == null)
            {
               Client throw new OauthException("{0} was not found.".Fill(OAuthTokenSecretKey));
            }

            var client = RequestFactory.NewClient();
            client.BaseUrl = AccClient     }

            return new Oauth1AccessToken(newParameters[OAuthTokenKey], newParamet_secret = parameters[OAuthTokenSecretKey];
   ClientConfiguration.ClientId, ClientConfiguration.ClientSecret, token, _secret, verifierLoginEndpoint.BaseUri        request.Method = Method.POST;

            var response = client.Execute(request);
       quest.AddParameter("state", state);
            }
    s(this.BuildLoginRequestUriParameters(parameters, ClientConfiguration, state));

            return client.BuildUri(request).ToString();
        }

        protected virtual NameValueCollection BuildLoginRequestUriParameters(NameValueCollection parameters, IClientConfiguration configuration, string state)
        {
            var loginRequestUriParameters = new NameValueCollection {{OAuthTokenKey, parameters[OAuthTokenKey]}};
            if (!state.IsEmpty())
            {
                loginRequestUriParameters.Add("state", state);
            }
            return loginRequestUriParametersith callback issued by provider.</param>
        /// <param name="verifier">Verifier posted with callback issued by provider.</param>
        /// <returns>Access token and other extra info.</returns>
        private NameValueCollection GetAccessToken(string token, string verifier)
        {
            var client = RequestFactory.NewClient();
            client.BaseUrl = AccessTokenEndpoint.BaseUri;
            client.Authenticator = OAuth1Authenticator.ForAccessToken(
                ClientConfiguration.ClientId, ClientConfiguration.ClientSecret, token, _secret, verifier);

            var request = RequestFactory.NewRequest();
            request.Resource = AccessTokenEndpoint.Resource;
            request.Method = Method.POST;

            var response = client.Execute(request);
            return HttpUtility.ParseQueryString(response.Content);
        }

        protected virtual UserInfo ParseUserInfo(string content)
        {
            throw new NotImplementedException();
        }

   ameters(parameters);

            return parameters;
        }

        /// <summary>
        /// Composes login link URI.
        /// </summary>
        /// <            
            return parameters