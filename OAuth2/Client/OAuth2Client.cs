using System;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;
using RestSharp;
using RestSharp.Contrib;
usmespace OAuth2.Client
{
    /// <summary>
    /// Base class for OAuth2 client implementation.
    /// </summary>
    public abstract class OAuth2Client : IClient
    {
        private const string AccessTokenKey = "access_token";

  

        protected readonly IRequestFactory RequestFactory;

        protected readonly IClientConfiguration ClientConfiguration;

        protected Endpoint AccessCodeEndpoint { get; set; }

        protected Endpoint AccessTokenEndpoint { get; set; }

        protected Endpoint AccessUserInfoEndpoint { get; set; }

        protected Func<string, UserInfo> UserInfoParser { get; set; }

        protected OAuth2Client(string name, Endpoint accessCodeEndpoint, Endpoint accessTokenEndpoint,
                               Endpoint userInfoEndpoint, IRequestFactory requestFactory,
                               IClientConfiguration clientConfiguration,
                               Func<string, UserInfo> userInfoParser)
        {
            Name = name;
            AccessCodeEndpoint = accessCodeEndpoint;
            AccessTokenEndpoint = accessTokenEndpoint;
            AccessUserInfoEndpoint = userInfoEndpoint;

            RequestFactory = requestFactory;
            ClientConfiguration = clientConfiguration;

            UserInfoParser = userInfoParser;
        }

        #region ICLient impl

        public string Name { get; protected s       public string GetLoginLinkUri(string state = null)
        {
            var client = _factory.NewRequestFactory.NewClient();
            client.BaseUrl = AccessCodeEndpoint.BaseUri;

            var request = RequestFactory.NewRequest();
            request.Resource = AccessCodeEndpoint.Resource;
            request.AddObject(this.BuildRequestTokenExchangeObject(ClientCe));

            return client.BuildUri(request).ToString();
        }

        public virtual OauthAccessToken Finalize(NameValueCollection parameters)
        {
            if (!parameters["error"].IsEmpty())
                throw new Exception(pa{
                throw new Exception(parameters["error"]);
            }

            var client = RequestFactory.NewClient();
            client.BaseUrl = AccessTokenEndpoint.BaseUri;

            var request = RequestFactory.NewRequest();
            request.Resource = AccessTokenequest.Method = Method.POST;
            request.AddObject(this.BuildAccessTokenExchangeObject(parameters, _configuration));

            varClientCesponse = client.Execute(request);
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

 if (oauth2AccessToken == null)
            {
                throw new OauthException("Oauth2AccessToken can not be null");
            }
            
            var client = RequestFactory.NewClient();
            client.BaseUrl = AccessUserInfoEndpoint.BaseUri;
            client.Authenticator = GetRequestAuthenticator(oauth2AccessToken);

            var request = RequestF;

            return client.Execute(request);
        }

        public UserInfo GetUserInfo(OauthAccessToken accessToken)
        {
            var restResponse = GetData(accessToken, UserInfoServiceEndpoint.Resource);

            var userInfo = ParseAccessUserInfoEndpoint.Resource);

            var userInfo = UserInfoParser(restResponse.Content);
            userInfo.ProviderName =     #region Private methods

        protected virtual IAuthenticator GetAuthenticator(Oauth2AccessToken accessToken)
        {
       dynamic BuildRequestTokenExchangeObject(IClientConfiguration configuration,       response_type = "code",
                    client_id = _configuration.ClientId,
                    redirect_uri = _configuration.RedirectUri,
   ClientConfiguration.ClientId,
                    redirect_uri = ClientConfiguration.RedirectUri,
                    scope = ClientC virtual dynamic BuildAccessTokenExchangeObject(NameValueCollection parameters,
                                                                 IClientConfiguration configurati         code = parameters["code"],
                    client_id = configuration.ClientId,
                    client_secret = configuration.ClientSecret,
                    redirect_uri = configuration.RedirectUri,
                    grant_type = "authorization_code"
                };
        }

        /// <summary>
        /// Should return parsed <see cref="UserInfo"/> using content received from provider.
   protected virtual IAuthenticator GetRequesAuth2UriQueryParameterAuthenticator(accessToken.Token);
        }

        protected virtual dynamic BuildRequestTokenExchangeObject(IClientConfiguration config#endregion
    }
}