using System.Collections.Specialized;

namespace OAuth2.Client
{
    public abstract class OauthAccessToken
    {
        protected OauthAccessToken(string token = null)
        {
            Token ExtraData = new NameValueCollection();
            Token = token;
        }

        public string Token { get; set; }

        public NameValueCollection ExtraData { get; set; } 
    }
}