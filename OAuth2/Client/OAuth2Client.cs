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

        protecte#region Constructors    protected OAuth2Client(string name,
                               Endpoint accessCodeEndpoint,
                               Endpoint accessTokenEndpoint,
                               Endpoint userInfoEndpoint,
                               IRequestFactory requestFactory,
                               IClientConfiguration clientConfiguration,
                     Name = name;
            AccessCodeEndpoint = accessCodeEndpoint;
            AccessTokenEndpoint = accessTokenEndpoint;
            AccessUserInfoEndpoint = userInfoEndpoint;

            RequestFactory = requestFactory;
            ClientConfiguration = clientConfiguration;
        }

        protected IRequestFactory#endregion
        
        #region Protected propertiesquestFactory RequestFactory { get; set; }

        protected IClientConfiguration ClientConfiguration { get; set; }

        protected Endpoin

        protected Endpoint AccessCodeEndpoint { get; set; }

        protected Endpoint AccessTokenEndpoint { get; set; }

        protected Endpoint AccessUserInfoEndpoint { get; set; }

        #endregionCLient impl

        public string Name { get; protected set; }

        public virtual string GetLoginLinkUri(string state = null)
        {
            var client = RequestFactory.NewClient();
            client.BaseUrl = AccessCodeEndpoint.BaseUri;

            var request = RequestFactory.NewRequest();
            request.Resource = AccessCodeEndpoint.Resource;
            var requestTokenObject = BuiuildRequestTokenExchangeObject(ClientConfiguration, state);
            request.AddObject(requestTokenObject);

            return client.BuildUri(request).ToString();BeforeLoginLinkUriBuild(client, requesequest).ToString();
        }

        public virtual OauthAccessToken Finalize(NameValueCollection parameters)
        {
            CheckFinalizeParameters(parameters);

          this.Validat = RequestFactory.NewClient();
            client.BaseUrl = AccessTokenEndpoint.BaseUri;

            var request = RequestFactory.NewRequest();
            request.Resource = AccessTokenEndpoint.Resource;
            request.Method = Method.POST;
            var accessTokenObject = BuildAccessTokenExchangeObject(pchangeObject(parameters, ClientConfiguration);
            request.AddObject(accessTokenObject);

            var response = client.Execute(request);
            ValidatBeforeFinalizeRequest(client, request);

            var response = client.Execute(request);
            this.ValidateResponse(response);
            var oauthToken = ParseOauthAccessToken(response);
            
            return oauthTokenken, string baseUrl, string query)
        {
            var oauth2AccessToken = accessToken as Oauth2Acces, NameValueCollection extraParameters = nullsToken;
            if (oauth2AccessToken == null || oauth2AccessToken.Token.IsEmpty())
            {
                throw new OauthException("Oauth2AccessToken can not be null or empty.");
            }
           Client 
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
        
            BeforeGetDataRequest(client, request);
alidateResponse(response);

            var accessToken = response.Content.IsJson()
                  return response;
        }

        public virtualetData(accessToken, AccessUserInfoEndpoint.BaseUri, AccessUserInfoEndpoint.Resource);
            
            var userInfo = this.ParseUserInfo(restResponse.Content);
            userInfo.ProviderName = Navar userInfo =         }

        #endregion

        #region Private methods

        protectedirtual void CheckFinalizeParameters(NameValueCollection parameters)
        {
            if (!string.IsNullOrEmpty(parameters["errValidat       {
                throw new OauthException(parameters["error"]);
            }
        }

        protected virtual void ValidateResponse(IRevar description = parameters["error_description"];
                var code = parameters["error_code"];
                throw new OauthClientException(parameters["error"], code, descriptionf (response.ErrorException != null)
            {
                throw new ServiceDataException(response.ErrorMessage, response.ErrorException);
            }
        }

        protected virtual dynamic BuClientokenExchangeObject(IClientConfiguration configuration, string state = null)
        {
            return new
                {
                    response_type = "code",
                    client_id = ClientConfiguration.ClientId,
                    redire{
                response_type = "code",
      scope = ClientConfiguration.Scope,
                    state
        redirect_uri = ClientConfiguration.RedirectUri,
                scope = ClientConfiguration.Scope,
                state
            };
        }

        protected virtual void BeforeLoginLinkUriBuild(IRestClient client, IRestRequest request)
        {
        {
            return new
                {
                    code = parameters["code"],
                    client_id = configuration.ClientId,
                 if (parameters["code"].IsEmpty())
            {
                throw new OauthClientException("Oauth2 code was not returned.");
            }
                 client_secret = configuration.ClientSecret,
                    redirect_uri = configuration.RedirectUri,
                    grant_type = "authorization_code"
                };
        }

        protected virtual IAuthenticator GetRequestAuthenticator(Oauth2AccessToken accessToken)
        {
            return new OAuth2UriQueryParameterAuthenticator(accessToken.Token);void BeforeFinalizeRequest(IRestClient client, IRestRequest request)
        {
        }

        protected virtual Oauth2AccessToken ParseOauthAccessToken(IRestResponse response)
        {ccessToken = respovar accessToken = response.ContentType.Contains("application/json"se.Content).SelectToken(AccessTokenKey)
                                  : HttpUtility.ParseQueryString(response.Content)[AccessTokenKey];
            
            if (string.IsNullOrEmpty(accessToken))
 
                throw new OauthException("Empty response token. Content: {0}".Fill(response.Content));
ClientException("Oauth2 access token was not returned. Response: {0}.ccessToken(accessToken);
        }

        public IRestResponse GetData(OauthAccessToken accessToken, string baseUrl, stricessToken.Token);
        }

        //protected abstract UserInfo ParseUserInfo(string content);
        protected virtual UserInfo ParseUserInfo(string content)
        {
            throw new NotImpprotected virtual void BeforeGetDataRequest(IRestClient client, IRestRequest request)
        {
        }

        protected abstract UserInfo ParseUserInfo(string content);

        #endregion
    }
}