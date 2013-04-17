using RestSharp;

namespace OAuth2.Client
{
    internal class NamedOAuth2UriQueryParameterAuthenticator : OAuth2Authenticator
    {
        public NamedOAuth2UriQueryParameterAuthenticator(string name, string accessToken)
            : base(accessToken)
        {
            this.Name = name;
        }

        private string Name { get; set; }

        public override void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddParameter(this.Name, this.AccessToken, ParameterType.GetOrPost);
        }
    }
}