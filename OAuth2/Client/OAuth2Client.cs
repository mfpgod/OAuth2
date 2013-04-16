using System;
using System.Collections.Specialized;

using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;
using RestSharp;
using RestSharp.Contrib;

namespace OAuth2.Client
{
    /// <summary>
    /// Base class for OAuth2 client implementation.
    /// </summary>
    public abstract class OAuth2Client : IClient
    {
        protected readonly string AccessTokenKey = "access_token";

        protected OAuth2Client(string name,
                               Endpoint accessCodeEndpoint,
                               Endpoint accessTokenEndpoint,
                               Endpoint userInfoEndpoint,
                               IRequestFactory requestFactory,
                               IClientConfiguration clientConfiguration,
                               Func<string, UserInfo> userInfoParser = null)
        {
            Name = name;
            AccessCodeEndpoint = accessCodeEndpoint;
            AccessTokenEndpoint = accessTokenEndpoint;
            AccessUserInfoEndpoint = userInfoEndpoint;

            RequestFactory = requestFactory;
            ClientConfiguration = clientConfiguration;
        }

        protected IRequestFactory RequestFactory { get; set; }

        protected IClientConfiguration ClientConfiguration { get; set; }

        protected Endpoint AccessCodeEndpoint { get; set; }

        protected Endpoint AccessTokenEndpoint { get; set; }

        protected Endpoint AccessUserInfoEndpoint { get; set; }
        
        #region ICLient impl

        public string Name { get; protected set; }

        public virtual string GetLoginLinkUri(string state = null)
        {
            var client = RequestFactory.NewClient();
            client.BaseUrl = AccessCodeEndpoint.BaseUri;

            var request = RequestFactory.NewRequest();
            request.Resource = AccessCodeEndpoint.Resource;
            var requestTokenObject = BuildRequestTokenExchangeObject(ClientConfiguration, state);
            request.AddObject(requestTokenObject);

            return client.BuildUri(request).ToString();
        }

        public virtual OauthAccessToken Finalize(NameValueCollection parameters)
        {
            CheckFinalizeParameters(parameters);

            var client = RequestFactory.NewClient();
            client.BaseUrl = AccessTokenEndpoint.BaseUri;

            var request = RequestFactory.NewRequest();
            request.Resource = AccessTokenEndpoint.Resource;
            request.Method = Method.POST;
            var accessTokenObject = BuildAccessTokenExchangeObject(parameters, ClientConfiguration);
            request.AddObject(accessTokenObject);

            var response = client.Execute(request);
            Validatreturn this.ParseOauthAccessToken(client.Execute(request));
        }

        protected virtual Oauth2AccessToken ParseOauthAccessToken(IRestResponse response)
        {ccessToken = response.Content.IsJson()
                                  ? (string)JObject.Parse(response.Content).SelectToken(AccessTokenKey)
                                  : HttpUtility.ParseQueryString(response.Content)[AccessTokenKey];
            
            if (string.IsNullOrEmpty(accessToken))
 
                throw new OauthException("Empty response token. Content: {0}".Fill(response.Content));
            }

            return new Oauth2AccessToken(accessToken);
        }

        public IRestResponse GetData(OauthAccessToken accessToken, string baseUrl, string query)
        {
            var oauth2AccessToken = accessToken as Oauth2Acces, NameValueCollection extraParameters = nullsToken;
            if (oauth2AccessToken == null || oauth2AccessToken.Token.IsEmpty())
            {
                throw new OauthException("Oauth2AccessToken can not be null or empty.");
            }
            
            var client = RequestFactory.NewClient();
            client.BaseUrl = baseUrl;
            client.Authenticator = GetRequestAuthenticator(oauth2AccessToken);

            var request = RequestFactory.NewRequest();
            request.AddResourceWithQuery(query);
            
            var response = client.Execute(request);
            ValidateResponse(responif (extraParameters != null)
            {
                request.AddParameters(extraParameters);
            }
                   ValidateResponse(response);

            var accessToken = response.Content.IsJson()
                return response;
        }

        public virtualetData(accessToken, AccessUserInfoEndpoint.BaseUri, AccessUserInfoEndpoint.Resource);
            
            var userInfo = this.ParseUserInfo(restResponse.Content);
            userInfo.ProviderName = Name;

            return userInfo;
        }

        #endregion

        #region Private methods

        protected virtual void CheckFinalizeParameters(NameValueCollection parameters)
        {
            if (!string.IsNullOrEmpty(parameters["error"]))
            {
                throw new OauthException(parameters["error"]);
            }
        }

        protected virtual void ValidateResponse(IRestResponse response)
        {
            if (response.ErrorException != null)
            {
                throw new ServiceDataException(response.ErrorMessage, response.ErrorException);
            }
        }

        protected virtual dynamic BuildRequestTokenExchangeObject(IClientConfiguration configuration, string state = null)
        {
            return new
                {
                    response_type = "code",
                    client_id = ClientConfiguration.ClientId,
                    redirect_uri = ClientConfiguration.RedirectUri,
                    scope = ClientConfiguration.Scope,
                    state
                };
        }

        protected virtual dynamic BuildAccessTokenExchangeObject(NameValueCollection parameters, IClientConfiguration configuration)
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

        protected virtual IAuthenticator GetRequestAuthenticator(Oauth2AccessToken accessToken)
        {
            return new OAuth2UriQueryParameterAuthenticator(accessToken.Token);
        }

        //protected abstract UserInfo ParseUserInfo(string content);
        protected virtual UserInfo ParseUserInfo(string content)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}