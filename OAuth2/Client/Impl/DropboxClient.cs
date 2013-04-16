using System.Collections.Specialized;

using Newtonsoft.Json.Linq;

using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespacusing RestSharpespace OAuth2.Client.Impl
{
    public class DropboxClient : OAuthClient
    {
        public static readonly string ClientName = "Dropbox";

        public static readonly Endpoint RequestTokenEndpoint = new Endpoint
            {
                BaseUri = "https://api.dropbox.com",
                Resource = "/1/oauth/request_token"
            };

        public static readonly Endpoint LoginEndpoint = new Endpoint
            {
                BaseUri = "https://www.dropbox.com",
                Resource = "/1/oauth/authorize"
            };

        public static readonly Endpoint TokenEndpoint = new Endpoint
            {
                BaseUri = "https://api.dropbox.com",
                Resource = "/1/oauth/access_token"
            };

        public static readonly Endpoint UserInfoEndpoint = new Endpoint
            {
                BaseUri = "https://api.dropbox.com",
                Resource = "/1/account/info"
            };

        public DropboxClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(ClientName, RequestTokenEndpoint, LoginEndpoint, TokenEndpoint, UserInfoEndpoint, factory, configuration)
        {
        }

        protected override NameValueCollection BuildLoginRequestUriParameters(NameValueCollection parameters, IClientConfiguration configuration, string state)
        {
            var loginRequestUriParameters = base.BuildLoginRequestUriParameters(parameters, configuration, state);
            loginRequestUriParameters.Add("oauth_callback", configuration.RedirectUri);
            return loginRequestUriParameters;
        }

        protected override UserInfo ParseUserInfo(string content)
        {
 void ValidateResponse(IRestResponse response)
        {
            base.ValidateResponse(response);
            if (response.Content.IsJson())
            {
                dynamic data = JObject.Parse(response.Content);
                if (data.error != null)
                {
                    throw new ServiceDataException(data.error.message.ToString(), data.error.type.ToString(), data.error.code.ToString());
                }
            } ParseUserInfo(string content)
        {
            dynamic response = JObject.Parse(content);

            var name = response.display_name.ToString();
            var index = name.IndexOf(' ');
            string firstName;
            string lastName;
            if (index == -1)
            {
                firstName = name;
                lastName = null;
            }
            else
            {
                firstName = name.Substring(0, index);
                lastName = name.Substring(index + 1);
            }
            
            return new UserInfo
            {
                Id = response.uid,
                Email = response.email,
                FirstName = firstName,
                LastName = lastName
            };
        }
    }
}