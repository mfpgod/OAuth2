namespace OAuth2.Models
{
    public class Oauth1AccessToken : OauthAccessToken
    {
        public Oauth1AccessToken(string token = null, string tokenSecret = null) : base(token)
        {
            TokenSecret = tokenSecret;
        }

        public string TokenSecret { get; set; } 
    }
    public class Oauth2AccessToken : OauthAccessToken
    {
        public Oauth2AccessToken(string token = null) : base(token)
        {
        }
    }
    public abstract class OauthAccessToken
    {
        protected OauthAccessToken(string token = null)
        {
            Token = token;
        }

        public string Token { get; set; } 
    }
}